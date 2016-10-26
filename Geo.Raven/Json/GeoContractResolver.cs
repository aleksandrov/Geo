﻿using System;
using System.Reflection;
using Geo.Abstractions.Interfaces;
using Raven.Imports.Newtonsoft.Json.Serialization;

namespace Geo.Raven.Json
{
    public class GeoContractResolver : DefaultContractResolver
    {
        private readonly Assembly _assembly = typeof(Coordinate).GetTypeInfo().Assembly;

        protected override JsonProperty CreateProperty(MemberInfo member, global::Raven.Imports.Newtonsoft.Json.MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);

            if(member.DeclaringType != null)
            {
                if (member.DeclaringType.GetTypeInfo().Assembly == _assembly)
                {
                    if (!prop.Writable)
                    {
                        var property = member as PropertyInfo;
                        if (property != null)
                        {
                            prop.Writable = property.GetSetMethod(true) != null;
                        }
                    }  
                }
            }

            return prop;
        }

        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            var contract = base.CreateObjectContract(objectType);

            if (typeof(ISpatial4nShape).IsAssignableFrom(objectType))
            {
                contract.Properties.Add(new JsonProperty
                {
                    Readable = true,
                    ShouldSerialize = value => true,
                    PropertyName = SpatialField.Name,
                    PropertyType = typeof(string),
                    Converter = ResolveContractConverter(typeof(string)),
                    ValueProvider = new RavenIndexStringProvider()
                });
            }

            return contract;
        }
    }
}
