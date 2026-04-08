using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia
{
    public interface ILoadingReceiver
    {
        public class Data
        {
            private readonly ILoadingReceiver _receiver;

            public string Description { get; private set; }
            public float Progress { get; private set; }

            public Data(ILoadingReceiver receiver)
            {
                _receiver = receiver;
            }

            public void SetDescription(string description)
            {
                Description = description;

                _receiver.OnLoadingDescriptionChanged(this);
            }

            public void SetProgress(float progress)
            {
                Progress = progress;

                _receiver.OnLoadingProgressChanged(this);
            }
        }

        void OnLoadingDescriptionChanged(Data data);

        void OnLoadingProgressChanged(Data data);
    }
}
