using Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Lorn.ADSP.Core.AdEngine.Abstractions.Extensions
{
    /// <summary>
    /// 默认回调提供者实现
    /// </summary>
    internal class DefaultCallbackProvider : ICallbackProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, IAdEngineCallback> _namedCallbacks = new();

        public DefaultCallbackProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T GetCallback<T>() where T : class, IAdEngineCallback
        {
            var callback = _serviceProvider.GetService<T>();
            if (callback == null)
            {
                throw new Exceptions.CallbackNotFoundException(typeof(T));
            }
            return callback;
        }

        public T GetCallback<T>(string name) where T : class, IAdEngineCallback
        {
            if (_namedCallbacks.TryGetValue(name, out var namedCallback) && namedCallback is T typedCallback)
            {
                return typedCallback;
            }

            throw new Exceptions.CallbackNotFoundException(name, typeof(T));
        }

        public bool HasCallback<T>() where T : class, IAdEngineCallback
        {
            return _serviceProvider.GetService<T>() != null;
        }

        public bool HasCallback(string callbackName)
        {
            return _namedCallbacks.ContainsKey(callbackName);
        }

        public IReadOnlyDictionary<string, IAdEngineCallback> GetAllCallbacks()
        {
            return _namedCallbacks.AsReadOnly();
        }

        public bool TryGetCallback<T>(out T? callback) where T : class, IAdEngineCallback
        {
            callback = _serviceProvider.GetService<T>();
            return callback != null;
        }

        public bool TryGetCallback<T>(string name, out T? callback) where T : class, IAdEngineCallback
        {
            callback = null;
            if (_namedCallbacks.TryGetValue(name, out var namedCallback) && namedCallback is T typedCallback)
            {
                callback = typedCallback;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 注册命名回调
        /// </summary>
        public void RegisterCallback(string name, IAdEngineCallback callback)
        {
            _namedCallbacks[name] = callback;
        }
    }
}
