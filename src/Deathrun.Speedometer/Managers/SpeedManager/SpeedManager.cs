using System.Globalization;
using Deathrun.Speedometer.Interfaces.Managers.SpeedManager;
using Sharp.Shared;
using Sharp.Shared.HookParams;
using Sharp.Shared.Listeners;
using Sharp.Shared.Managers;
using Sharp.Shared.Objects;

namespace Deathrun.Speedometer.Managers.SpeedManager;

internal class SpeedManager(
    IModSharp modSharp,
    IHookManager hookManager) : ISpeedManager, IGameListener
{
    private static IGlobalVars? _globalVars = null;
    
    #region IModule
    
    public bool Init()
    {
        hookManager.PlayerPostThink.InstallForward(PlayerPostThink);
        
        modSharp.InstallGameListener(this);
        
        return true;
    }

    public static void OnPostInit() { }

    public void Shutdown()
    {
        hookManager.PlayerPostThink.RemoveForward(PlayerPostThink);
        
        modSharp.RemoveGameListener(this);
    }

    #endregion

    #region Hooks

    private static void PlayerPostThink(IPlayerPawnFunctionParams parms)
    {
        if (Speedometer.DeathrunManagerApi?.Instance is not { } deathrunManagerApi) return;
        
        if (_globalVars?.TickCount % 3 is not 0) return;
        
        var deathrunPlayer = deathrunManagerApi.Managers.PlayersManager.GetDeathrunPlayer(parms.Client);
        if (deathrunPlayer is null) return;

        var speedNum = deathrunPlayer.PlayerPawn?.GetAbsVelocity().Length() ?? 0;
            
        deathrunPlayer.SetCenterMenuMiddleRowHtml
        (
            $"<font class='fontSize-m stratum-font fontWeight-Bold' color='#A7A7A7'>Speed: </font>"
            + $"<font class='fontSize-m stratum-font fontWeight-Bold' color='magenta'>{speedNum.ToString("F", CultureInfo.InvariantCulture)}</font>"    
        );
    }

    #endregion
    
    #region Listeners

    public void OnServerInit() => _globalVars = modSharp.GetGlobals();
    
    #endregion
    
    int IGameListener.ListenerVersion => IGameListener.ApiVersion;
    int IGameListener.ListenerPriority => 8;
}




