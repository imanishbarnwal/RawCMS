﻿//******************************************************************************
// <copyright file="license.md" company="RawCMS project  (https://github.com/arduosoft/RawCMS)">
// Copyright (c) 2019 RawCMS project  (https://github.com/arduosoft/RawCMS)
// RawCMS project is released under GPL3 terms, see LICENSE file on repository root at  https://github.com/arduosoft/RawCMS .
// </copyright>
// <author>Daniele Fontani, Emanuele Bucarelli, Francesco Min�</author>
// <autogenerated>true</autogenerated>
//******************************************************************************
using GraphQL;
using GraphQL.Types;
using Newtonsoft.Json.Linq;
using RawCMS.Library.Schema;
using RawCMS.Plugins.GraphQL.Classes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RawCMS.Plugins.GraphQL.Types
{
    public class CollectionType : ObjectGraphType<object>
    {
        public QueryArguments TableArgs
        {
            get; set;
        }

        private IDictionary<FieldBaseType, Type> _fieldTypeToSystemType;

        protected IDictionary<FieldBaseType, Type> FieldTypeToSystemType
        {
            get
            {
                if (_fieldTypeToSystemType == null)
                {
                    _fieldTypeToSystemType = new Dictionary<FieldBaseType, Type>
                    {
                        { FieldBaseType.Boolean, typeof(bool) },
                        { FieldBaseType.Date, typeof(DateTime) },
                        { FieldBaseType.Float, typeof(float) },
                        { FieldBaseType.ID, typeof(Guid) },
                        { FieldBaseType.Int, typeof(int) },
                        { FieldBaseType.String, typeof(string) },
                        { FieldBaseType.Object, typeof(JObject) }
                    };
                }

                return _fieldTypeToSystemType;
            }
        }

        private Type ResolveFieldMetaType(FieldBaseType type)
        {
            if (FieldTypeToSystemType.ContainsKey(type))
            {
                return FieldTypeToSystemType[type];
            }

            return typeof(string);
        }

        public CollectionType(CollectionSchema collectionSchema, Dictionary<string, CollectionSchema> collections = null, GraphQLService graphQLService = null)
        {
            Name = collectionSchema.CollectionName;

            foreach (Field field in collectionSchema.FieldSettings)
            {
                InitGraphField(field, collections, graphQLService);
            }
        }

        private void InitGraphField(Field field, Dictionary<string, CollectionSchema> collections = null, GraphQLService graphQLService = null)
        {
            Type graphQLType;
            if (field.BaseType == FieldBaseType.Object)
            {
                var relatedObject = collections[field.Type];
                var relatedCollection = new CollectionType(relatedObject, collections);
                var listType = new ListGraphType(relatedCollection);
                graphQLType = relatedCollection.GetType();
                FieldType columnField = Field(
                graphQLType,
                relatedObject.CollectionName);

                columnField.Resolver = new NameFieldResolver();
                columnField.Arguments = new QueryArguments(relatedCollection.TableArgs);
                foreach (var arg in columnField.Arguments.Where(x => !(new string[] { "pageNumber", "pageSize", "rawQuery", "_id" }.Contains(x.Name))).ToList())
                {
                    arg.Name = $"{relatedObject.CollectionName}_{arg.Name}";
                    TableArgs.Add(arg);
                }
            }
            else
            {
                //graphQLType = (ResolveFieldMetaType(field.BaseType)).GetGraphTypeFromType(!field.Required);
                graphQLType = (ResolveFieldMetaType(field.BaseType)).GetGraphTypeFromType(true);
                FieldType columnField = Field(
                graphQLType,
                field.Name);

                columnField.Resolver = new NameFieldResolver();
                FillArgs(field.Name, graphQLType);
            }
        }

        private void FillArgs(string name, Type graphType)
        {
            if (TableArgs == null)
            {
                TableArgs = new QueryArguments(
                    new QueryArgument(graphType)
                    {
                        Name = name
                    }
                );
            }
            else
            {
                TableArgs.Add(new QueryArgument(graphType) { Name = name });
            }

            TableArgs.Add(new QueryArgument<IntGraphType> { Name = "pageNumber" });
            TableArgs.Add(new QueryArgument<IntGraphType> { Name = "pageSize" });
            TableArgs.Add(new QueryArgument<StringGraphType> { Name = "rawQuery" });
        }
    }
}