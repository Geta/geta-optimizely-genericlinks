using EPiServer.DataAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Geta.Optimizely.GenericLinks.Tests.Services
{
    public class InMemoryPropertyDefinitionTypeRepository : IPropertyDefinitionTypeRepository
    {
        private readonly IDictionary<int, PropertyDefinitionType> _definitionsById;
        private readonly IDictionary<string, PropertyDefinitionType> _definitionsByTypeAndAssembly;
        private readonly IDictionary<Type, PropertyDefinitionType> _definitionsByType;
        private readonly ISet<PropertyDefinitionType> _definitions;

        public InMemoryPropertyDefinitionTypeRepository(IEnumerable<PropertyDefinitionType> propertyDefinitions)
        {
            _definitionsById = propertyDefinitions.ToDictionary(x => x.ID);
            _definitionsByTypeAndAssembly = propertyDefinitions.ToDictionary(x => GetTypeAndAssemblyName(x.TypeName, x.AssemblyName));
            _definitionsByType = propertyDefinitions.ToDictionary(x => x.DefinitionType);
            _definitions = propertyDefinitions.ToHashSet();
        }

        public void Delete(int id)
        {
            if (!_definitionsById.TryGetValue(id, out var definition))
                return;

            Delete(definition);
        }

        public virtual void Delete(PropertyDefinitionType definition)
        {
            var typeName = GetTypeAndAssemblyName(definition.TypeName, definition.AssemblyName);

            _definitionsById.Remove(definition.ID);
            _definitionsByType.Remove(definition.DefinitionType);
            _definitionsByTypeAndAssembly.Remove(typeName);
            _definitions.Remove(definition);
        }

        public IEnumerable<PropertyDefinitionType> List()
        {
            return _definitions;
        }

        public PropertyDefinitionType Load(int id)
        {
            if (_definitionsById.TryGetValue(id, out var definition))
                return definition;

            throw new InvalidOperationException($"PropertyDefinition with id {id} not found");
        }

        public PropertyDefinitionType Load(string typeName, string assemblyName)
        {
            var typeAndAssembly = GetTypeAndAssemblyName(typeName, assemblyName);

            if (_definitionsByTypeAndAssembly.TryGetValue(typeAndAssembly, out var definition))
                return definition;

            throw new InvalidOperationException($"PropertyDefinition with type {typeName} and assembly {assemblyName} not found");
        }

        public PropertyDefinitionType Load(Type type)
        {
            if (_definitionsByType.TryGetValue(type, out var definition))
                return definition;

            throw new InvalidOperationException($"PropertyDefinition with type {type.Name} not found");
        }

        public BlockPropertyDefinitionType LoadByBlockType(Guid blockTypeId)
        {
            throw new NotImplementedException();
        }

        public BlockPropertyDefinitionType LoadByBlockType(Type blockTypeModel)
        {
            throw new NotImplementedException();
        }

        public void Save(PropertyDefinitionType propertyDefinitionType)
        {
            throw new NotImplementedException();
        }

        protected virtual string GetTypeAndAssemblyName(string type, string assembly)
        {
            return $"{type},{assembly}";
        }

        protected virtual Guid GetGuid(Guid? guid, Type type)
        {
            if (guid is not null)
                return guid.Value;

            return type.GUID;
        }
    }
}
