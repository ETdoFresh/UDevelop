using System;
using Cysharp.Threading.Tasks;

namespace GameEditor.Databases
{
    public interface IDatabase
    {
        void AddValueChangedListener(string path, EventHandler<IValueChangedEventArgs> listener);
        void RemoveValueChangedListener(string path, EventHandler<IValueChangedEventArgs> listener);
        void AddChildAddedListener(string path, EventHandler<IChildChangedEventArgs> listener);
        void RemoveChildAddedListener(string path, EventHandler<IChildChangedEventArgs> listener);
        void AddChildRemovedListener(string path, EventHandler<IChildChangedEventArgs> listener);
        void RemoveChildRemovedListener(string path, EventHandler<IChildChangedEventArgs> listener);
        void AddChildChangedListener(string path, EventHandler<IChildChangedEventArgs> listener);
        void RemoveChildChangedListener(string path, EventHandler<IChildChangedEventArgs> listener);
        void AddChildMovedListener(string path, EventHandler<IChildChangedEventArgs> listener);
        void RemoveChildMovedListener(string path, EventHandler<IChildChangedEventArgs> listener);
        void AddObjectChild(string path, string key, object value);
        void RemoveObjectChild(string path, string key);
        void AddArrayChild(string path, object value);
        void RemoveArrayChild(string path, int index);
        UniTask SetValueAsync(string path, object value);
        UniTask<object> GetValueAsync(string path);
    }
}