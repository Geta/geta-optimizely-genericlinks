using EPiServer.Shell.ObjectEditing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Geta.Optimizely.GenericLinks.Tests.Services
{
    public class NullEditorDefinitionRepository : IEditorDefinitionRepository
    {
        public void Add(EditorDefinition editorDefinition)
        {
        }

        public void Delete(EditorDefinition editorDefinition)
        {
        }

        public EditorDefinition? Get(Type type, string uiHint)
        {
            return null;
        }

        public IEnumerable<EditorDefinition> List()
        {
            return Enumerable.Empty<EditorDefinition>();
        }

        public void Update(EditorDefinition updatedValue)
        {
        }
    }
}
