using System;
using System.Collections.Generic;
using System.Globalization;
using Deathrun.Speedometer.Interfaces.Managers.SpeedManager;
using DeathrunManager.Shared.Objects;
using Sharp.Shared;
using Sharp.Shared.GameEntities;
using Sharp.Shared.HookParams;
using Sharp.Shared.Listeners;
using Sharp.Shared.Managers;
using Sharp.Shared.Objects;

namespace Deathrun.Speedometer.Managers.SpeedManager;

internal class SpeedManager(
    IModSharp modSharp) : ISpeedManager, IGameListener
{
    private static IGlobalVars? _globalVars = null;
    
    #region IModule
    
    public bool Init()
    {
        modSharp.InstallGameFrameHook(null, OnGameFramePost);
        modSharp.InstallGameListener(this);
        
        return true;
    }

    public static void OnPostInit() { }

    public void Shutdown()
    {
        modSharp.RemoveGameFrameHook(null, OnGameFramePost);
        modSharp.RemoveGameListener(this);
    }

    #endregion

    #region Hooks

    private readonly List<IDeathrunPlayer> _deathrunPlayersBuffer = new(64);

    private void OnGameFramePost(bool simulating, bool bFirstTick, bool bLastTick)
    {
        if (Speedometer.DeathrunManagerApi?.Instance is not { } deathrunManagerApi) return;

        deathrunManagerApi.Managers.PlayersManager.GetAllValidDeathrunPlayersZAlloc(_deathrunPlayersBuffer);

        foreach (var deathrunPlayer in _deathrunPlayersBuffer)
        {
            float speedNum = 0;
            if (deathrunPlayer.PlayerPawn?.IsAlive is true)
            {
                speedNum = deathrunPlayer.PlayerPawn?.GetAbsVelocity().Length() ?? 999;
            }
            else
            {
                var observedDeathrunPlayer = deathrunPlayer.ObservedDeathrunPlayer;
                if (observedDeathrunPlayer is null) continue;
                
                speedNum = observedDeathrunPlayer.PlayerPawn?.GetAbsVelocity().Length() ?? 777;
            }
            
            deathrunPlayer.SetCenterMenuMiddleRowHtml
            (
                $"<font class='fontSize-m stratum-font fontWeight-Bold' color='#A7A7A7'>Speed: </font>"
                + $"<font class='fontSize-m stratum-font fontWeight-Bold' color='magenta'>{speedNum.ToString("F", CultureInfo.InvariantCulture)}</font>"    
            );
        }
    }
    
    #endregion
    
    #region Listeners

    public void OnServerInit() => _globalVars = modSharp.GetGlobals();
    
    #endregion
    
    int IGameListener.ListenerVersion => IGameListener.ApiVersion;
    int IGameListener.ListenerPriority => 8;
}




