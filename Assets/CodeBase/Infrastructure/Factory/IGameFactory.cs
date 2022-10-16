using System;
using System.Collections.Generic;
using CodeBase.Infrastructure.Services;
using CodeBase.Infrastructure.Services.PersistentProgress;
using UnityEngine;

namespace CodeBase.Infrastructure.Factory
{
    public interface IGameFactory : IService
    {
        List<ISavedProgressReader> ProgressReaders { get; }
        List<ISavedProgress> ProgressesWriters { get; }
        GameObject HeroGameObject { get; set; }
        event Action HeroCreated;
        GameObject CreateHero(GameObject at);
        GameObject CreateHud();
        void Cleanup();
        void Register(ISavedProgressReader progressReader);
    }
}