// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using EPiServer.Core.Transfer;
using EPiServer.Data;
using EPiServer.Data.Entity;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Geta.Optimizely.GenericLinks
{
    public abstract class LinkDataCollection
    {

    }

    public class LinkDataCollection<TLinkData> : LinkDataCollection, IList<TLinkData>, IReadOnly<LinkDataCollection<TLinkData>>, ICloneable, IReferenceMap
        where TLinkData : ILinkData
    {
        private readonly List<TLinkData> _linkCollection;

        private bool _isReadOnly;
        private bool _isModified;

        public LinkDataCollection()
        {
            _linkCollection = new List<TLinkData>();
        }

        public LinkDataCollection(int capacity)
        {
            _linkCollection = new List<TLinkData>(capacity);
        }

        public LinkDataCollection(IEnumerable<TLinkData> links)
        {
            _linkCollection = new List<TLinkData>(links);
        }

        protected LinkDataCollection(bool isModified, bool isReadOnly, List<TLinkData> links)
        {
            _isModified = isModified;
            _isReadOnly = isReadOnly;
            _linkCollection = links;
        }

        public virtual TLinkData this[int index]
        {
            get => _linkCollection[index];
            set
            {
                ThrowIfReadOnly();

                _linkCollection[index] = value;
                _isModified = true;
            }
        }

        public virtual int Count => _linkCollection.Count;
        public virtual bool IsReadOnly => _isReadOnly;
        public virtual bool IsModified
        {
            get
            {
                if (_isModified)
                    return true;

                if (_linkCollection is null)
                    return false;

                foreach (var item in _linkCollection)
                {
                    if (item is not LinkData linkData)
                        continue;

                    if (linkData.IsModified)
                        return true;
                }

                return false;
            }
        }

        public virtual IList<Guid> ReferencedPermanentLinkIds
        {
            get
            {
                var linkIds = new List<Guid>(_linkCollection.Count);

                foreach (var item in _linkCollection)
                    linkIds.AddRange(item.ReferencedPermanentLinkIds);

                return linkIds;
            }
        }

        public virtual void Add(TLinkData item)
        {
            _linkCollection.Add(item);
            _isModified = true;
        }

        public virtual void Clear()
        {
            if (_linkCollection.Count < 1)
                return;

            _linkCollection.Clear();
            _isModified = true;
        }

        public virtual bool Contains(TLinkData item) => _linkCollection.Contains(item);
        public virtual void CopyTo(TLinkData[] array, int arrayIndex) => _linkCollection.CopyTo(array, arrayIndex);
        public virtual int IndexOf(TLinkData item) => _linkCollection.IndexOf(item);
        public virtual void Insert(int index, TLinkData item)
        {
            _linkCollection.Insert(index, item);
            _isModified = true;
        }
        public virtual void MakeReadOnly()
        {
            _isReadOnly = true;
            _isModified = false;

            if (_linkCollection is null)
                return;

            foreach (var item in _linkCollection)
            {
                if (item is not LinkData linkData)
                    continue;

                if (linkData.IsModified)
                    linkData.SetModified(false);
            }
        }

        public virtual bool Remove(TLinkData item)
        {
            var removed = _linkCollection.Remove(item);
            if (removed)
                _isModified = true;

            return removed;
        }

        public virtual void RemoveAt(int index)
        {
            _linkCollection.RemoveAt(index);
            _isModified = true;
        }

        public virtual void RemapPermanentLinkReferences(IDictionary<Guid, Guid> idMap)
        {
            foreach (var link in _linkCollection)
            {
                link.RemapPermanentLinkReferences(idMap);
            }
        }

        public virtual IEnumerator<TLinkData> GetEnumerator() => _linkCollection.GetEnumerator();

        public virtual LinkDataCollection<TLinkData> CreateWritableClone()
        {
            return Clone(false);
        }

        public virtual object Clone()
        {
            return Clone(_isReadOnly);
        }

        protected virtual LinkDataCollection<TLinkData> Clone(bool isReadOnly)
        {
            var list = new List<TLinkData>(_linkCollection.Count);
            foreach (var item in _linkCollection)
            {
                list.Add((TLinkData)item.Clone());
            }

            return new LinkDataCollection<TLinkData>(_isModified, isReadOnly, list);
        }

        protected virtual void ThrowIfReadOnly()
        {
            Validator.ValidateNotReadOnly(this);
        }

        object IReadOnly.CreateWritableClone() => CreateWritableClone();
        IEnumerator IEnumerable.GetEnumerator() => _linkCollection.GetEnumerator();
    }
}
