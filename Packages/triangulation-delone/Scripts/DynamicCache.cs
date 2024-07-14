using UnityEngine;

namespace Delone
{
	internal class DynamicCache
    {
        private Triangle[] _cache;
 
        private uint _size = 2;
 
        private uint _inCache;
 
        private readonly Vector2 _min;
        private readonly Vector2 _max;
 
        private float _xSize;
        private float _ySize;
 
        public DynamicCache(Vector2 min, Vector2 max)
        {
            _min = min;
            _max = max;
            Reset();
        }

        public void Reset()
        {
            _size = 2;
            _inCache = 0;
            _xSize = (_max.x - _min.x) / _size;
            _ySize = (_max.y - _min.y) / _size;
 
            _cache = new Triangle[_size * _size];
        }
 
        public void Add(Triangle triangle)
        {
            _inCache++;
 
            if (_inCache >= _cache.Length * 3)
                Increase();

            var key = GetKey(triangle.Centroid);
            _cache[key] = triangle;
        }
 
        public Triangle FindTriangle(Vector2 point)
        {
            uint key = GetKey(point);
            if (_cache[key] != null)
                return _cache[key];
 
            for (uint i = key - 25; i < key && i >= 0 && i < _cache.Length; i++)
                if (_cache[i] != null)
                    return _cache[i];
 
            for (uint i = key + 25; i > key && i >= 0 && i < _cache.Length; i--)
                if (_cache[i] != null)
                    return _cache[i];
 
            return null;
        }
 
        private void Increase()
        {
            var newSize = _size * 2;
            
            Triangle[] newCache = new Triangle[newSize * newSize];

            for (uint i = 0; i < _cache.Length; i++)
            {
                var newIndex = GetNewIndex(i);
                newCache[newIndex] = _cache[i];
                newCache[newIndex + 1] = _cache[i];
                newCache[newIndex + newSize] = _cache[i];
                newCache[newIndex + newSize + 1] = _cache[i];
            }
 
            _size = newSize;
            _xSize = (_max.x - _min.x) / _size;
            _ySize = (_max.y - _min.y) / _size;
 
            _cache = newCache;
        }
 
        private uint GetKey(Vector2 point)
        {
            uint i = (uint)((point.y - _min.y) / _ySize);
            uint j = (uint)((point.x - _min.x) / _xSize);
 
            if (i == _size)
                i--;
            if (j == _size)
                j--;
 
            return i * _size + j;
        }
 
        private uint GetNewIndex(uint oldIndex)
        {
            uint i = (oldIndex / _size) * 2;
            uint j = (oldIndex % _size) * 2;
 
            return i * (_size * 2) + j;
        }
    }
}