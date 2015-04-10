using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VitML.JsonVM.Linq;

namespace VitML.JsonVM
{
    public class JTokenVMPathReader
    {

        private JTokenVM tokenVM;

        public JTokenVMPathReader(JTokenVM tokenVM)
        {
            this.tokenVM = tokenVM;
        }

        public object GetValue(string path)
        {
            StringBuilder nameBuffer = new StringBuilder();
            string propName = null;
            int index = 0;
            bool hasIndexer = false;
            object result = null;
            JTokenVM obj = this.tokenVM;
            for (int pos = 0; pos <= path.Length; pos++)
            {
                bool isEOF = pos == path.Length;
                char ch = (char)0;
                if (!isEOF)
                    ch = path[pos];
                if (isEOF || ch == '.')
                {
                    propName = nameBuffer.ToString();
                    nameBuffer.Clear();
                    if (obj is JArrayVM)
                    {
                        throw new Exception("illegal array call");
                    }
                    else if (obj is JObjectVM)
                    {
                        JPropertyVM prop = (obj as JObjectVM).Properties.FirstOrDefault(x => x.Key == propName);
                        if (!hasIndexer)
                        {
                            if (isEOF)
                                result = prop;
                            else
                                obj = prop.Value as JTokenVM;
                        }
                        else
                        {
                            hasIndexer = false;
                            obj = prop.Value as JTokenVM;
                            if (obj is JArrayVM)
                            {
                                object value = (obj as JArrayVM).Data[index];
                                if (isEOF)
                                    result = value;
                                else
                                {
                                    if (value is JTokenVM)
                                        obj = value as JTokenVM;
                                    else
                                        throw new Exception("is not a JTokenVM item but a " + value.GetType());
                                }
                            }
                            else
                            {
                                throw new Exception("cant call indexer on JObjectVM");
                            }
                        }
                    }
                    continue;
                }
                else if (ch == '[')
                {
                    index = ReadIntIndexer(path, pos + 1, ref pos);
                    hasIndexer = true;
                    continue;
                }
                nameBuffer.Append(ch);
            }
            return result;
        }

        private int ReadIntIndexer(string path, int startPos, ref int endPos)
        {
            StringBuilder buffer = new StringBuilder();
            for (int pos = startPos; pos < path.Length; pos++)
            {
                char ch = path[pos];
                bool isLastChar = pos == path.Length - 1;
                if (ch == ']' || isLastChar)
                {
                    string index = buffer.ToString();
                    buffer.Clear();
                    int indexInt = 0;
                    if (int.TryParse(index, out indexInt))
                    {
                        endPos = pos;
                        return indexInt;
                    }
                    else
                    {
                        throw new NotImplementedException("Path dict not supperted");
                    }
                }
                buffer.Append(ch);
            }
            throw new Exception("unclosed indexer");
        }
    }
}
