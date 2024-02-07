// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using EPiServer.Shell.ObjectEditing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Geta.Optimizely.GenericLinks.Tests.Services;

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
