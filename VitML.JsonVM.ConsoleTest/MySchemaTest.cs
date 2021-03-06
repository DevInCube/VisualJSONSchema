﻿using My.Json.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace VitML.JsonVM.ConsoleTest
{
    class MySchemaTest
    {

        public static void Run()
        {
            var json = @"{
    '$schema':'http://json-schema.org/draft-04/schema#',
    'id':'http://vit.com.ua/edgeserver/compositor#',

    'type':'object',
	'format':'tab',

	'definitions' : {
		
	},

    'properties':{

        'Source':{

            'id':'Source',
            'type':'object',

			'description' : 'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industrys standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.',


            'properties':{

                'MasterName':{
                    'id':'MasterName',
                    'type': ['string', 'null'],
					'format':'label',
					'description' : 'Lorem Ipsum is simply dummy text of the printing and',
                },

				'SetMaster':{
                    'type':'string',
                    'format':'button',
					'ignore':'true',
					'default':'Set as Master',
					'description' : 'Lorem Ipsum is simply dummy text of the printing and',
                },

                'QueueSet':{
                    'id':'QueueSet',
                    'type':'array',

					'description' : 'Lorem Ipsum is simply dummy text of the printing and',

					'format' : 'list',

					'Style' : { 
						'MaxHeight' : 450,
						'MinHeight' : 450,
					},

                    'items': {
                        'type':'object',


						'Style' : { 
							'DisplayMemberPath' : 'Name',
						},

                        'properties':{
                            'Name':{
                                'id':'Name',
                                'type':'string',
								'default':'name',
                            },
                            'Prefix':{
                                'id':'Prefix',
                                'type':'string'
                            },
                            'Suffix':{
                                'id':'Suffix',
                                'type':'string'
                            },
                            'Dir':{
                                'id':'Dir',
                                'type':'string'
                            },
                            'Remove':{
                                'id':'Remove',
                                'type':'boolean',
								'description' : 'Lorem Ipsum is simply dummy text of the printing and',
                            },
                            'Sink':{
                                'id':'Sink',
                                'type':'boolean'
                            },
                            'Tout':{
                                'id':'Tout',
                                'type':'integer'
                            },
                        },

						'required':[ 'Name', 'Prefix', 'Suffix', 'Dir', 'Remove', 'Sink', 'Tout' ],
						'additionalProperties': false,
                    }
                    
                }
            },
            'required':[ 'MasterName', 'QueueSet' ],
			'additionalProperties': false,
        },

        'Sink':{
            'id':'Sink',
            'type':'object',

			'description' : 'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industrys standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.',

            'properties':{
                'QueueSet':{
                    'id':'QueueSet',
                    'type':'array',

					'format' : 'list',

					'Style' : {
						'MaxHeight' : 300,
						'MinHeight' : 300,
					},
                    
					'items': { '$ref' : 'definitions#/definitions/EventDir' },
                }
            },

			'required':[ 'QueueSet' ],
			'additionalProperties': false,
        }
    },

    'required':[
        'Source',
        'Sink'
    ],
	'additionalProperties': false,
}";
            JSchemaPreloadedResolver res0 = new JSchemaPreloadedResolver();
            res0.Add(new Uri("http://vit.com.ua/edgeserver/definitions"), File.ReadAllText("Resources/common/definitions.txt"));
            JSchema sh111 = JSchema.Parse(json, res0);
            return;
        }
    }
}
