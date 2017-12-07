using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAS.Extensions
{
    public static class ObservableCollectionExtensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>
        (this IEnumerable<T> source)
        {
            if (source == null)
                source = new ObservableCollection<T>();
            
            return new ObservableCollection<T>(source);
        }
    }
}
