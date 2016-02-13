using System.Collections.Generic;

namespace ResotelApp.Utils
{
    public class BiDirectionalIterator<T> where T : class
    {
        private List<T> _elements;
        private int _index=0;

        public BiDirectionalIterator(List<T> elements)
        {
            _elements = elements;
        }

        public T Current {
            get
            {
                return _elements.Count > 0 ? _elements[_index] : null;
            }
        }

        public bool HasNext
        {
            get { return (_index < _elements.Count - 1) && _elements.Count > 0; }
        }

        public bool HasPrev
        {
            get { return _index > 0 && _elements.Count > 0; }
        }

        public void MoveNext()
        {
            if(HasNext)
            {
                _index++;
            }
        }

        public void MovePrev()
        {
            if(HasPrev)
            {
                _index--;
            }
        }
    }
}
