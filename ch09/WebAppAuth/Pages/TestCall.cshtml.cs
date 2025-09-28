using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Http.Logging;

using WebAppAuth.Auth;

namespace WebAppAuth.Pages;

public class TestCallModel(GatewayClient client) : PageModel
{
    public async Task OnGetAsync()
    {
        await client.TestAsync();
    }
}
