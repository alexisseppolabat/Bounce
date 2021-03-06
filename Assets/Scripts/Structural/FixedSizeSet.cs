using System.Collections.Generic;

namespace Structural {
    public class FixedSizeSet<T> : HashSet<T> {
        private readonly int maxSize;
        private int size;

        public FixedSizeSet(int maxSize) {
            this.maxSize = maxSize;
        }
    
        public new bool Add(T t){
            // Only allow another element to be added to the set
            // if the set's maximum size has not been reached
            if (size >= maxSize) return false;
            bool result = base.Add(t);
            size  = result ? size + 1 : size;
            return result;
        }

        public new bool Remove(T t) {
            bool result = base.Remove(t);
            size  = result ? size - 1 : size;
            return result;
        }

        public new void ExceptWith(IEnumerable<T> e) {
            base.ExceptWith(e);
            size = Count;
        }
    }
}