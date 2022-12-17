using System;

namespace CodeBase.Infrastructure.Services.Ads
{
    public interface IAdsService
    {
        event Action RewardedVideoReady;
        bool IsRewardedVideoReady { get; }
        void Initialize();
        void ShowRewardedVideo(Action onVideoFinished);
    }
}