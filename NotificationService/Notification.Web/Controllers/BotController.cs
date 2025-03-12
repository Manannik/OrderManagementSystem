using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Notification.Web.Controllers;

[ApiController]
public class BotController(
    IUpdateHandler updateHandler,
    ITelegramBotClient botClient) : Controller
{
    [HttpPost("/webhook/update")]
    public async Task<IActionResult> Index([FromBody] Update update)
    {
        try
        {
            await updateHandler.HandleUpdateAsync(botClient, update, CancellationToken.None);
        }
        catch (Exception ex)
        {
            await updateHandler.HandleErrorAsync(botClient, ex, HandleErrorSource.PollingError, CancellationToken.None);
        }

        return Ok();
    }

    [HttpGet("Ping")]
    public IActionResult Ping()
    {
        return Ok("Ok");
    }
}