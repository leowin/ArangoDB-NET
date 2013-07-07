﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Arango.Client
{
    public class Document : Dictionary<string, object>
    {
        public Document() {}
        
        public Document(string json) 
        {
            Deserialize(json);
        }
        
        #region Field operations
        
        public T GetField<T>(string fieldPath)
        {
            Type type = typeof(T);
            T value;

            if (type.IsPrimitive || type.IsArray || (type.Name == "String"))
            {
                value = default(T);
            }
            else
            {
                value = (T)Activator.CreateInstance(type);
            }

            if (fieldPath.Contains("."))
            {
                var fields = fieldPath.Split('.');
                int iteration = 1;
                Document embeddedDocument = this;

                foreach (var field in fields)
                {
                    if (iteration == fields.Length)
                    {
                        // if value is collection type, get element type and enumerate over its elements
                        if (value is IList)
                        {
                            Type elementType = ((IEnumerable)value).GetType().GetGenericArguments()[0];
                            IEnumerator enumerator = ((IEnumerable)embeddedDocument[field]).GetEnumerator();

                            while (enumerator.MoveNext())
                            {
                                // if current element is ODocument type which is dictionary<string, object>
                                // map its dictionary data to element instance
                                if (enumerator.Current is Document)
                                {
                                    var instance = Activator.CreateInstance(elementType);
                                    ((Document)enumerator.Current).Map(ref instance);

                                    ((IList)value).Add(instance);
                                }
                                else
                                {
                                    ((IList)value).Add(Convert.ChangeType(enumerator.Current, elementType));
                                }
                            }
                        }
                        else if (type.Name == "HashSet`1")
                        {
                            Type elementType = ((IEnumerable)value).GetType().GetGenericArguments()[0];
                            IEnumerator enumerator = ((IEnumerable)this[fieldPath]).GetEnumerator();

                            var addMethod = type.GetMethod("Add");

                            while (enumerator.MoveNext())
                            {
                                // if current element is ODocument type which is Dictionary<string, object>
                                // map its dictionary data to element instance
                                if (enumerator.Current is Document)
                                {
                                    var instance = Activator.CreateInstance(elementType);
                                    ((Document)enumerator.Current).Map(ref instance);

                                    addMethod.Invoke(value, new object[] { instance });
                                }
                                else
                                {
                                    addMethod.Invoke(value, new object[] { enumerator.Current });
                                }
                            }
                        }
                        else if (type.IsEnum)
                        {
                            value = (T)Enum.ToObject(type, embeddedDocument[field]);
                        }
                        else
                        {
                            if (embeddedDocument[field] is T) 
                            {
                                value = (T)embeddedDocument[field];
                            } 
                            else
                            {
                                return (T)Convert.ChangeType(embeddedDocument[field], typeof(T));
                            }
                        }
                        break;
                    }

                    if (embeddedDocument.ContainsKey(field))
                    {
                        embeddedDocument = (Document)embeddedDocument[field];
                        iteration++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                if (this.ContainsKey(fieldPath))
                {
                    // if value is list or set type, get element type and enumerate over its elements
                    if (value is IList)
                    {
                        Type elementType = ((IEnumerable)value).GetType().GetGenericArguments()[0];
                        IEnumerator enumerator = ((IEnumerable)this[fieldPath]).GetEnumerator();

                        while (enumerator.MoveNext())
                        {
                            // if current element is ODocument type which is Dictionary<string, object>
                            // map its dictionary data to element instance
                            if (enumerator.Current is Document)
                            {
                                var instance = Activator.CreateInstance(elementType);
                                ((Document)enumerator.Current).Map(ref instance);

                                ((IList)value).Add(instance);
                            }
                            else
                            {
                                ((IList)value).Add(Convert.ChangeType(enumerator.Current, elementType));
                            }
                        }
                    }
                    else if (type.Name == "HashSet`1")
                    {
                        Type elementType = ((IEnumerable)value).GetType().GetGenericArguments()[0];
                        IEnumerator enumerator = ((IEnumerable)this[fieldPath]).GetEnumerator();

                        var addMethod = type.GetMethod("Add");

                        while (enumerator.MoveNext())
                        {
                            // if current element is ODocument type which is Dictionary<string, object>
                            // map its dictionary data to element instance
                            if (enumerator.Current is Document)
                            {
                                var instance = Activator.CreateInstance(elementType);
                                ((Document)enumerator.Current).Map(ref instance);

                                addMethod.Invoke(value, new object[] { instance });
                            }
                            else
                            {
                                addMethod.Invoke(value, new object[] { enumerator.Current });
                            }
                        }
                    }
                    else if (type.IsEnum)
                    {
                        value = (T)Enum.ToObject(type, this[fieldPath]);
                    }
                    else
                    {
                        if (this[fieldPath] is T) 
                        {
                            value = (T)this[fieldPath];
                        } 
                        else
                        {
                            return (T)Convert.ChangeType(this[fieldPath], typeof(T));
                        }
                    }
                }
            }

            return value;
        }

        public Document SetField<T>(string fieldPath, T value)
        {
            if (fieldPath.Contains("."))
            {
                var fields = fieldPath.Split('.');
                int iteration = 1;
                Document embeddedDocument = this;

                foreach (var field in fields)
                {
                    if (iteration == fields.Length)
                    {
                        if (embeddedDocument.ContainsKey(field))
                        {
                            embeddedDocument[field] = value;
                        }
                        else
                        {
                            embeddedDocument.Add(field, value);
                        }
                        break;
                    }

                    if (embeddedDocument.ContainsKey(field))
                    {
                        embeddedDocument = (Document)embeddedDocument[field];
                    }
                    else
                    {
                        // if document which contains the field doesn't exist create it first
                        Document tempDocument = new Document();
                        embeddedDocument.Add(field, tempDocument);
                        embeddedDocument = tempDocument;
                    }

                    iteration++;
                }
            }
            else
            {
                if (this.ContainsKey(fieldPath))
                {
                    this[fieldPath] = value;
                }
                else
                {
                    this.Add(fieldPath, value);
                }
            }

            return this;
        }
        
        public Document RemoveField(string fieldPath)
        {
            if (fieldPath.Contains("."))
            {
                var fields = fieldPath.Split('.');
                int iteration = 1;
                Document embeddedDocument = this;

                foreach (var field in fields)
                {
                    if (iteration == fields.Length)
                    {
                        if (embeddedDocument.ContainsKey(field))
                        {
                            embeddedDocument.Remove(field);
                        }
                        break;
                    }

                    if (embeddedDocument.ContainsKey(field))
                    {
                        embeddedDocument = (Document)embeddedDocument[field];
                        iteration++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                if (this.ContainsKey(fieldPath))
                {
                    this.Remove(fieldPath);
                }
            }
            
            return this;
        }

        public bool HasField(string fieldPath)
        {
            bool contains = false;

            if (fieldPath.Contains("."))
            {
                var fields = fieldPath.Split('.');
                int iteration = 1;
                Document embeddedDocument = this;

                foreach (var field in fields)
                {
                    if (iteration == fields.Length)
                    {
                        contains = embeddedDocument.ContainsKey(field);
                        break;
                    }

                    if (embeddedDocument.ContainsKey(field))
                    {
                        embeddedDocument = (Document)embeddedDocument[field];
                        iteration++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                contains = this.ContainsKey(fieldPath);
            }

            return contains;
        }
        
        #endregion
        
        // maps ArangoDocument fields to specified object
        private void Map(ref object obj)
        {
            if (obj is Dictionary<string, object>)
            {
                obj = this;
            }
            else
            {
                Type objType = obj.GetType();

                foreach (KeyValuePair<string, object> item in this)
                {
                    PropertyInfo property = objType.GetProperty(item.Key);

                    if (property != null)
                    {
                        property.SetValue(obj, item.Value, null);
                    }
                }
            }
        }
        
        public Document Except(params string[] fields)
        {
            Document document = new Document();
            
            foreach (KeyValuePair<string, object> field in this)
            {
                if (!fields.Contains(field.Key))
                {
                    document.Add(field.Key, field.Value);
                }
            }
            
            return document;
        }
        
        #region Serialization

        public static string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        
        #endregion
        
        #region Deserialization

        public void Deserialize(string json)
        {
            Dictionary<string, JToken> fields = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(json);

            foreach (KeyValuePair<string, JToken> field in fields)
            {
                switch (field.Value.Type)
                {
                    case JTokenType.Array:
                        this.Add(field.Key, DeserializeArray((JArray)field.Value));
                        break;
                    case JTokenType.Object:
                        this.Add(field.Key, DeserializeEmbeddedObject((JObject)field.Value));
                        break;
                    default:
                        this.Add(field.Key, DeserializeValue(field.Value));
                        break;
                }
            }
        }

        private object DeserializeEmbeddedObject(JObject jObject)
        {
            Document embedded = new Document();

            foreach (KeyValuePair<string, JToken> field in jObject)
            {
                switch (field.Value.Type)
                {
                    case JTokenType.Array:
                        embedded.Add(field.Key, DeserializeArray((JArray)field.Value));
                        break;
                    case JTokenType.Object:
                        embedded.Add(field.Key, DeserializeEmbeddedObject((JObject)field.Value));
                        break;
                    default:
                        embedded.Add(field.Key, DeserializeValue(field.Value));
                        break;
                }
            }

            return embedded;
        }

        private List<object> DeserializeArray(JArray jArray)
        {
            List<object> array = new List<object>();
            
            foreach (JToken item in jArray)
            {
                switch (item.Type)
                {
                    case JTokenType.Array:
                        array.Add(DeserializeArray((JArray)item));
                        break;
                    case JTokenType.Object:
                        array.Add(DeserializeEmbeddedObject((JObject)item));
                        break;
                    default:
                        array.Add(DeserializeValue(item));
                        break;
                }
            }
            
            return array;
        }
        
        private object DeserializeValue(JToken token)
        {
            return token.ToObject<object>();
        }
        
        #endregion
        
        #region Convert to generic object
        
        public T To<T>() where T : class, new()
        {
            T genericObject = new T();

            genericObject = (T)ToObject<T>(genericObject, this);

            return genericObject;
        }
        
        private T ToObject<T>(T genericObject, Document document) where T : class, new()
        {
            var genericObjectType = genericObject.GetType();

            if (genericObjectType.Name.Equals("ArangoDocument") ||
                genericObjectType.Name.Equals("ArangoEdge"))
            {
                // if generic object is arango specific class - use set field to copy data
                foreach (KeyValuePair<string, object> item in document)
                {
                    (genericObject as Document).SetField(item.Key, item.Value);
                }
            }
            else
            {
                foreach (PropertyInfo propertyInfo in genericObjectType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    var propertyName = propertyInfo.Name;
                    var arangoProperty = propertyInfo.GetCustomAttribute<ArangoProperty>();
                    object fieldValue;
                    
                    if (arangoProperty != null)
                    {
                        if (!arangoProperty.Serializable)
                        {
                            continue;
                        }
                        
                        if (!string.IsNullOrEmpty(arangoProperty.Alias))
                        {
                            propertyName = arangoProperty.Alias;
                        }
                    }
                    
                    if (document.HasField(propertyName))
                    {
                        fieldValue = document.GetField<object>(propertyName);
                    }
                    else
                    {
                        continue;
                    }
                    
                    if ((propertyInfo.PropertyType.IsArray || propertyInfo.PropertyType.IsGenericType))
                    {
                        var collection = (IList)fieldValue;

                        if (collection.Count > 0)
                        {
                            // create instance of property type
                            var collectionInstance = Activator.CreateInstance(propertyInfo.PropertyType, collection.Count);

                            for (int i = 0; i < collection.Count; i++)
                            {
                                // collection is simple array
                                if (propertyInfo.PropertyType.IsArray)
                                {
                                    ((object[])collectionInstance)[i] = collection[i];
                                }
                                // collection is generic
                                else if (propertyInfo.PropertyType.IsGenericType && (fieldValue is IEnumerable))
                                {
                                    var elementType = collection[i].GetType();

                                    // generic collection consists of basic types or ORIDs
                                    if (elementType.IsPrimitive ||
                                        (elementType == typeof(string)) ||
                                        (elementType == typeof(DateTime)) ||
                                        (elementType == typeof(decimal)))
                                    {
                                        ((IList)collectionInstance).Add(collection[i]);
                                    }
                                    // generic collection consists of generic type which should be parsed
                                    else
                                    {
                                        // create instance object based on first element of generic collection
                                        var instance = Activator.CreateInstance(propertyInfo.PropertyType.GetGenericArguments().First(), null);
                                        
                                        if (instance.GetType() == typeof(Document))
                                        {
                                            ((IList)collectionInstance).Add(ToObject(instance, (Document)fieldValue));
                                        }
                                        else
                                        {
                                            ((IList)collectionInstance).Add(collection[i]);
                                        }
                                    }
                                }
                                else
                                {
                                    var v = Activator.CreateInstance(collection[i].GetType(), collection[i]);

                                    ((IList)collectionInstance).Add(v);
                                }
                            }

                            propertyInfo.SetValue(genericObject, collectionInstance, null);
                        }
                    }
                    // property is class except the string type since string values are parsed differently
                    else if (propertyInfo.PropertyType.IsClass &&
                        (propertyInfo.PropertyType.Name != "String"))
                    {
                        // create object instance of embedded class
                        var instance = Activator.CreateInstance(propertyInfo.PropertyType);

                        if (propertyInfo.PropertyType == typeof(Document))
                        {
                            propertyInfo.SetValue(genericObject, ToObject(instance, (Document)fieldValue), null);
                        }
                        else
                        {
                            propertyInfo.SetValue(genericObject, fieldValue, null);
                        }
                    }
                    // property is basic type
                    else
                    {
                        propertyInfo.SetValue(genericObject, fieldValue, null);
                    }
                }
            }

            return genericObject;
        }
        
        #endregion
        
        #region Convert from generic object
        
        public void From<T>(T genericObject)
        {
            foreach (KeyValuePair<string, object> field in FromObject(genericObject))
            {
                this.Add(field.Key, field.Value);
            }
        }
        
        private Document FromObject<T>(T genericObject)
        {
            var document = new Document();
            var genericObjectType = genericObject.GetType();
            
            foreach (PropertyInfo propertyInfo in genericObjectType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var propertyName = propertyInfo.Name;
                var arangoProperty = propertyInfo.GetCustomAttribute<ArangoProperty>();

                if (arangoProperty != null)
                {
                    if (!arangoProperty.Serializable)
                    {
                        continue;
                    }
                    
                    if (!string.IsNullOrEmpty(arangoProperty.Alias))
                    {
                        propertyName = arangoProperty.Alias;
                    }
                }
                
                var propertyValue = propertyInfo.GetValue(genericObject);
                
                if ((propertyInfo.PropertyType.IsArray || propertyInfo.PropertyType.IsGenericType))
                {
                    var collection = (IList)propertyValue;

                    if (collection.Count > 0)
                    {
                        // create instance of property type
                        var collectionInstance = Activator.CreateInstance(propertyInfo.PropertyType, collection.Count);

                        for (int i = 0; i < collection.Count; i++)
                        {
                            // collection is simple array
                            if (propertyInfo.PropertyType.IsArray)
                            {
                                ((object[])collectionInstance)[i] = collection[i];
                            }
                            // collection is generic
                            else if (propertyInfo.PropertyType.IsGenericType && (propertyValue is IEnumerable))
                            {
                                var elementType = collection[i].GetType();

                                // generic collection consists of basic types or ORIDs
                                if (elementType.IsPrimitive ||
                                    (elementType == typeof(string)) ||
                                    (elementType == typeof(DateTime)) ||
                                    (elementType == typeof(decimal)))
                                {
                                    ((IList)collectionInstance).Add(collection[i]);
                                }
                                // generic collection consists of generic type which should be parsed
                                else
                                {
                                    // create instance object based on first element of generic collection
                                    var instance = Activator.CreateInstance(propertyInfo.PropertyType.GetGenericArguments().First(), null);
                                    
                                    if (instance.GetType() == typeof(Document))
                                    {
                                        ((IList)collectionInstance).Add(FromObject(instance));
                                    }
                                    else
                                    {
                                        ((IList)collectionInstance).Add(collection[i]);
                                    }
                                }
                            }
                            else
                            {
                                var v = Activator.CreateInstance(collection[i].GetType(), collection[i]);

                                ((IList)collectionInstance).Add(v);
                            }
                        }

                        //propertyInfo.SetValue(genericObject, collectionInstance, null);
                        document.SetField(propertyName, collectionInstance);
                    }
                }
                // property is class except the string type since string values are parsed differently
                else if (propertyInfo.PropertyType.IsClass &&
                    (propertyInfo.PropertyType.Name != "String"))
                {
                    // create object instance of embedded class
                    //var instance = Activator.CreateInstance(propertyInfo.PropertyType);

                    if (propertyInfo.PropertyType == typeof(Document))
                    {
                        //propertyInfo.SetValue(genericObject, ToObject(instance, (Document)fieldValue), null);
                        document.SetField(propertyName, FromObject(propertyValue));
                    }
                    else
                    {
                        //propertyInfo.SetValue(genericObject, fieldValue, null);
                        document.SetField(propertyName, propertyValue);
                    }
                }
                // property is basic type
                else
                {
                    //propertyInfo.SetValue(genericObject, fieldValue, null);
                    document.SetField(propertyName, propertyValue);
                }
            }
            
            return document;
        }
        
        #endregion
    }
}
