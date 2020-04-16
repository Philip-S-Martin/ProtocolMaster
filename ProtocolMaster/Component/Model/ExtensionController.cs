using ProtocolMaster.Component.Debug;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProtocolMaster.Component.Model
{
    /*
    public class ExtensionController
    {
        public readonly string name;

        Lazy<ManagedExtension> extension;
        Dictionary<string, Type> compatibilityDictionary;

        public Task testTask;
        public CancellationToken cancelToken;
        private CancellationTokenSource tokenSource;
        private ConcurrentQueue<Event> data;
        
        public ExtensionController(Lazy<ManagedExtension> extension)
        {
            this.extension = extension;
            var metadata = extension.Metadata;
            if (metadata.Symbol.Length == 0)
            {
                Log.Error("Extension Symbol is empty");
                return;
            }
            // Assign name
            if (metadata.Symbol[0].GetType() == typeof(string))
            {
                this.name = (string)metadata.Symbol[0];
            }
            else
            {
                Log.Error("Extension Symbol[0] is not a string, Name invalid");
                return;
            }

            // Assign compatibility
            compatibilityDictionary = new Dictionary<string, Type>();
            for (int i = 1; i < metadata.Symbol.Length; i += 2)
            {
                if (metadata.Symbol.Length >= i + 1 && metadata.Symbol[i].GetType() == typeof(string) && metadata.Symbol[i + 1].GetType() == typeof(Type))
                {
                    compatibilityDictionary.Add((string)metadata.Symbol[i], metadata.Symbol[i + 1].GetType());
                }
                else
                {
                    Log.Error("Extension Symbol[" + i + "] or its associeated type is invalid or not present)");
                    continue;
                }
            }
            data = new ConcurrentQueue<Event>();
        }
        ~ExtensionController()
        {
            Stop();
        }

        public CancellationTokenSource TokenSource { get => tokenSource; set => tokenSource = value; }

        public bool TryAddEvent(Event newEvent)
        {
            foreach(KeyValuePair<string, object> pair in newEvent.Properties)
            {
                if(compatibilityDictionary.ContainsKey(pair.Key))
                {
                    if (compatibilityDictionary[pair.Key] != pair.Value.GetType())
                        return false;
                }
                else return false;
            }
            data.Enqueue(newEvent);
            return true;
        }

        public void Start()
        {
            TokenSource = new CancellationTokenSource();
            cancelToken = TokenSource.Token;

            testTask = Task.Run(new Action(() =>
            {
                // Create the driver
                ManagedExtension ext = extension.Value;
                // Run driver setup
                ext.Begin();
                // Register driver cancel
                cancelToken.Register(new Action(() => ext.Cancel()));
                // pre-fill event data
                Event transfer;
                while (data.Count > 0)
                    if (data.TryDequeue(out transfer))
                        ext.ReadEvent(transfer);
                // Loop through driver
                while (ext.ShouldLoop())
                {
                    ext.Loop();
                    while (data.Count > 0)
                        if (data.TryDequeue(out transfer))
                            ext.ReadEvent(transfer);
                }
                // "Deconstruct" driver as needed
                ext.Done();
            }), TokenSource.Token);
        }

        public async void Stop()
        {
            TokenSource.Cancel();
            try
            {
                await testTask;
            }
            catch (OperationCanceledException)
            {
                Log.Out($"{nameof(OperationCanceledException)} thrown");
            }
            finally
            {
                TokenSource.Dispose();
            }
        }
    }
    */
}
