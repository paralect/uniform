using System;
using System.Collections.Generic;
using System.Reflection;

namespace Uniform.Documents
{
    public class Analyzer
    {
        private readonly Dictionary<Type, List<Dependent>> _map = new Dictionary<Type, List<Dependent>>();

        private readonly List<Type> _types;

        public Analyzer(List<Type> types)
        {
            _types = types;
        }

        public void Analyze()
        {
            foreach (var type in _types)
            {
                AnalyzeType(type);
            }
        }

        public void AnalyzeType(Type type)
        {
            var infos = type.GetProperties();

            foreach (var propertyInfo in infos)
            {
                if (!IsTrackedType(propertyInfo.PropertyType))
                    continue;

                var dep = new Dependent();
                dep.DependentType = type;
                dep.Path.Add(propertyInfo);

                var list = GetDependents(propertyInfo.PropertyType);
                list.Add(dep);
            }
        }

        public List<Dependent> GetDependents(Type type)
        {
            List<Dependent> value;
            if (!_map.TryGetValue(type, out value))
                _map[type] = value = new List<Dependent>();

            return value;
        }

        private bool IsTrackedType(Type type)
        {
            return _types.Contains(type);
        }
    }

    public class Dependent
    {
        public Type DependentType { get; set; }
        public List<PropertyInfo> Path { get; set; }

        public Dependent()
        {
            Path = new List<PropertyInfo>();
        }
    }
}