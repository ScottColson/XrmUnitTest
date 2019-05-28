﻿using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DLaB.Xrm.LocalCrm
{
    internal class EntityProperties
    {
        public Dictionary<string, PropertyInfo> PropertiesByName { get; private set; }
        public Dictionary<string,List<PropertyInfo>> PropertiesByLowerCaseName { get; private set; }
        public Dictionary<string, PropertyInfo> PropertiesByLogicalName { get; private set; }
        public string EntityName { get; private set; }

        public bool IsActivityType => PropertiesByName.ContainsKey("ActivityId");
        

        private EntityProperties()
        {
            PropertiesByName = new Dictionary<string, PropertyInfo>();
            PropertiesByLogicalName = new Dictionary<string, PropertyInfo>();
        }

        public bool ContainsProperty(string name)
        {
            return PropertiesByName.ContainsKey(name) 
                || PropertiesByLogicalName.ContainsKey(name);
        }

        public PropertyInfo GetProperty(string name)
        {
            if (PropertiesByName.TryGetValue(name, out PropertyInfo property) ||
                PropertiesByLogicalName.TryGetValue(name, out property))
            {
                return property;
            }
            throw new KeyNotFoundException($"The property \"{name}\" was not found in the entity type \"{EntityName}\".");
        }

        public static EntityProperties Get<T>() where T: Entity
        {
            return Get(typeof(T));
        }

        public static EntityProperties Get(Type type) 
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToDictionary(p => p.Name);
            
            var entity = new EntityProperties
            {
                EntityName = EntityHelper.GetEntityLogicalName(type),
                PropertiesByName = properties,
                PropertiesByLogicalName = properties.Values
                                                    .Select(p => new { Key = p.GetAttributeLogicalName(false), Property = p })
                                                    .Where(p => p.Key != null)
                                                    .GroupBy(k => k.Key, p => p.Property)
                                                    .Select(g => new { g.Key, Property = g.FirstOrDefault()})
                                                    .ToDictionary(k => k.Key, p => p.Property),
                PropertiesByLowerCaseName = properties.GroupBy(v => v.Key.ToLower(), v => v.Value).ToDictionary(v => v.Key, v => v.ToList())
            };

            return entity;
        }
    }
}
