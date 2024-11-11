//using BookQuoteAPI.Models;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;


//[ApiController]
//[Route("api/[controller]")]
//public class AuthController : ControllerBase
//{
//    private readonly UserManager<IdentityUser> _userManager;

//    public AuthController(UserManager<IdentityUser> userManager)
//    {
//        _userManager = userManager;
//    }

//    [HttpPost("register")]
//    public async Task<IActionResult> Register(UserRegistrationModel model)
//    {
//        if (!ModelState.IsValid)
//            return BadRequest(ModelState);

//        var user = new IdentityUser { UserName = model.Email, Email = model.Email };
//        var result = await _userManager.CreateAsync(user, model.Password);

//        if (result.Succeeded)
//        {
//            return Ok("User registered successfully");
//        }

//        foreach (var error in result.Errors)
//        {
//            ModelState.AddModelError(error.Code, error.Description);
//        }

//        return BadRequest(ModelState);
//    }
//}
