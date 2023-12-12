using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace ETdoFresh.Localbase
{
    public class Query
    {
        public class QueryEntry
        {
            public Query query;
            public DatabaseReference databaseReference; 
            public LocalbaseDatabase database;
            public string path;
            public Data<ValueChangedEventArgs> valueChanged;
            public Data<ChildChangedEventArgs> childAdded;
            public Data<ChildChangedEventArgs> childChanged;
            public Data<ChildChangedEventArgs> childRemoved;
            public Data<ChildChangedEventArgs> childMoved;
        }

        public static List<QueryEntry> QueryEntries = new();
        public QueryEntry queryEntry;
        
        public event EventHandler<ValueChangedEventArgs> ValueChanged
        {
            add => queryEntry.valueChanged.AddListener(value);
            remove => queryEntry.valueChanged.RemoveListener(value);
        }
        
        public event EventHandler<ChildChangedEventArgs> ChildAdded
        {
            add => queryEntry.childAdded.AddListener(value);
            remove => queryEntry.childAdded.RemoveListener(value);
        }
        
        public event EventHandler<ChildChangedEventArgs> ChildChanged
        {
            add => queryEntry.childChanged.AddListener(value);
            remove => queryEntry.childChanged.RemoveListener(value);
        }
        
        public event EventHandler<ChildChangedEventArgs> ChildRemoved
        {
            add => queryEntry.childRemoved.AddListener(value);
            remove => queryEntry.childRemoved.RemoveListener(value);
        }
        
        public event EventHandler<ChildChangedEventArgs> ChildMoved
        {
            add => queryEntry.childMoved.AddListener(value);
            remove => queryEntry.childMoved.RemoveListener(value);
        }
        
        public DatabaseReference Reference { get; set; }

        public async UniTask<DataSnapshot> GetValueAsync()
        {
            var tcs = new UniTaskCompletionSource<DataSnapshot>();
            EventHandler<ValueChangedEventArgs> listener = null;
            listener = (sender, args) =>
            {
                queryEntry.valueChanged.RemoveListener(listener);
                tcs.TrySetResult(args.Snapshot);
            };
            queryEntry.valueChanged.AddListener(listener);
            return await tcs.Task;
        }

        public object KeepSynced(bool keepSynced)
        {
            throw new NotImplementedException();
        }

        public Query StartAt(string value)
        {
            throw new NotImplementedException();
        }

        public Query StartAt(double value)
        {
            throw new NotImplementedException();
        }

        public Query StartAt(bool value)
        {
            throw new NotImplementedException();
        }

        public Query StartAt(string value, string key)
        {
            throw new NotImplementedException();
        }

        public Query StartAt(double value, string key)
        {
            throw new NotImplementedException();
        }

        public Query StartAt(bool value, string key)
        {
            throw new NotImplementedException();
        }

        public Query EndAt(string value)
        {
            throw new NotImplementedException();
        }

        public Query EndAt(double value)
        {
            throw new NotImplementedException();
        }

        public Query EndAt(bool value)
        {
            throw new NotImplementedException();
        }

        public Query EndAt(string value, string key)
        {
            throw new NotImplementedException();
        }

        public Query EndAt(double value, string key)
        {
            throw new NotImplementedException();
        }

        public Query EndAt(bool value, string key)
        {
            throw new NotImplementedException();
        }

        public Query EqualTo(string value)
        {
            throw new NotImplementedException();
        }

        public Query EqualTo(double value)
        {
            throw new NotImplementedException();
        }

        public Query EqualTo(bool value)
        {
            throw new NotImplementedException();
        }

        public Query EqualTo(string value, string key)
        {
            throw new NotImplementedException();
        }

        public Query EqualTo(double value, string key)
        {
            throw new NotImplementedException();
        }

        public Query EqualTo(bool value, string key)
        {
            throw new NotImplementedException();
        }

        public Query LimitToFirst(int limit)
        {
            throw new NotImplementedException();
        }

        public Query LimitToLast(int limit)
        {
            throw new NotImplementedException();
        }

        public Query OrderByChild(string path)
        {
            throw new NotImplementedException();
        }

        public Query OrderByPriority()
        {
            throw new NotImplementedException();
        }

        public Query OrderByKey()
        {
            throw new NotImplementedException();
        }

        public Query OrderByValue()
        {
            throw new NotImplementedException();
        }
    }
}