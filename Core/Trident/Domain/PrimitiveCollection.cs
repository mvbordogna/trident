using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Trident.Domain
{
    public class PrimitiveCollection<T> 
    {
        private const char delimiter = '|';
        private List<T> _list = new List<T>();

        [Key]
        public Guid Key { get; set; } = Guid.NewGuid();

        public string Raw
        {
            get
            {
                return (List?.Any() == true) ? string.Join(delimiter, this.List.Select(x => x.ToString())) : string.Empty;
            }
            protected set
            {
                if(value != null)
                {
                    var results = new List<T>();
                    var ary = value.Split(delimiter);
                    if (ary.Any())
                    {
                        foreach (var s in ary)
                        {
                            results.Add(s.ChangeType<T>());
                        }
                        this.List = results;
                    }                  
                }
            }
        }

        [NotMapped]
        public List<T> List
        {

            get { return _list; }
            set { _list = value; }
        }        

        [NotMapped] public T this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }
    }
}
