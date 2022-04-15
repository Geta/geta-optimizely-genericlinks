// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using System.Linq;
using EPiServer.DataAbstraction;

namespace Geta.Optimizely.GenericLinks.Tests.Services
{
    public class InMemoryFrameRepository : IFrameRepository
    {
        private readonly IDictionary<int, Frame> _framesById;
        private readonly IDictionary<string, Frame> _framesByName;

        public InMemoryFrameRepository(IEnumerable<Frame> frames)
        {
            _framesById = frames.ToDictionary(x => x.ID);
            _framesByName = frames.ToDictionary(x => x.Name);
        }

        public void Delete(int id)
        {
            if (!_framesById.TryGetValue(id, out var frame))
                return;

            _framesById.Remove(frame.ID);
            _framesByName.Remove(frame.Name);
        }

        public IEnumerable<Frame> List()
        {
            return _framesById.Values;
        }

        public Frame? Load(string? name)
        {
            if (name is null)
                return null;

            if (_framesByName.TryGetValue(name, out var frame))
                return frame;

            return null;
        }

        public Frame? Load(int id)
        {
            if (_framesById.TryGetValue(id, out var frame))
                return frame;

            return null;
        }

        public int Save(Frame? frame)
        {
            if (frame is null)
                return -1;

            if (_framesById.ContainsKey(frame.ID))
            {
                _framesById[frame.ID] = frame;
            }
            else
            {
                _framesById.Add(frame.ID, frame);
            }

            if (_framesByName.ContainsKey(frame.Name))
            {
                _framesByName[frame.Name] = frame;
            }
            else
            {
                _framesByName.Add(frame.Name, frame);
            }

            return frame.ID;
        }
    }
}
