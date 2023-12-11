using System;
using System.Threading.Tasks;
using GameEditor.Databases;

namespace ETdoFresh.Localbase
{
    public class Query
    {
        public event EventHandler<ValueChangedEventArgs> ValueChanged;
        public event EventHandler<ChildChangedEventArgs> ChildAdded;
        public event EventHandler<ChildChangedEventArgs> ChildChanged;
        public event EventHandler<ChildChangedEventArgs> ChildRemoved;
        public event EventHandler<ChildChangedEventArgs> ChildMoved;
        
        public DatabaseReference Reference { get; set; }

        public Task<DataSnapshot> GetValueAsync()
        {
            throw new NotImplementedException();
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