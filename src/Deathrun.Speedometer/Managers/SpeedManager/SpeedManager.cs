using System.Globalization;
using Deathrun.Speedometer.Interfaces.Managers.SpeedManager;
using Microsoft.Extensions.Logging;
using Sharp.Shared;
using Sharp.Shared.HookParams;

namespace Deathrun.Speedometer.Managers.SpeedManager;

internal class SpeedManager(
    ILogger<SpeedManager> logger,
    ISharedSystem sharedSystem) : ISpeedManager
{
    #region IModule
    
    public bool Init()
    {
        
        logger.LogInformation("[Deathrun][SpeedManager] {colorMessage}", "Load Speed Manager");
        
        sharedSystem.GetHookManager().PlayerPostThink.InstallForward(PlayerPostThink);
        
        return true;
    }

    public static void OnPostInit() { }

    public void Shutdown()
    {
        sharedSystem.GetHookManager().PlayerPostThink.RemoveForward(PlayerPostThink);
        
        logger.LogInformation("[Deathrun][SpeedManager] {colorMessage}", "Unload Speed Manager");
    }

    #endregion

    #region Hooks

    private void PlayerPostThink(IPlayerPawnFunctionParams parms)
    {
        if (sharedSystem.GetModSharp().GetGlobals().TickCount % 3 is not 0) return;
                
        if (Speedometer.DeathrunManagerApi?.Instance is { } deathrunManagerApi)
        {
            var client = parms.Client;
            if (client?.IsValid is not true) return;
            
            var deathrunPlayer = deathrunManagerApi.GetPlayersManager.GetDeathrunPlayer(client);
            if (deathrunPlayer is null) return;

            var speedNum = deathrunPlayer.PlayerPawn?.GetAbsVelocity().Length();
            
            deathrunPlayer.SetCenterMenuMiddleRowHtml
            (
                $"<font class='fontSize-m stratum-font fontWeight-Bold' color='#A7A7A7'>Speed: </font>"
                + $"<font class='fontSize-m stratum-font fontWeight-Bold' color='magenta'>{speedNum?.ToString("F", CultureInfo.InvariantCulture)}</font>"    
            );
        }
    }

    #endregion
    
}




