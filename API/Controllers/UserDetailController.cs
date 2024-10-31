using BusinessObjects;
using Firebase.Storage;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Interface;
using Services.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDetailController : ControllerBase
    {
        private readonly IUserDetailService _userDetailService;

        public UserDetailController(IUserDetailService userDetailService)
        {
            _userDetailService = userDetailService;
        }

        [HttpGet]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> GetPaginatedUserDetails(
            [FromQuery] string searchQuery = "",
            [FromQuery] string sortBy = "name_asc",
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var paginatedUserDetails = await _userDetailService.GetPaginatedUsers(searchQuery, sortBy, pageIndex, pageSize);
                return Ok(paginatedUserDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving paginated user details: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> GetUserDetailById(string id)
        {
            try
            {
                var userDetail = await _userDetailService.GetUserById(id);
                return Ok(userDetail);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving user detail by ID: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserDetail([FromForm] UserDetailModel userDetail, string id)
        {
            try
            {
                if (userDetail.ImageUrl != null)
                {

                    var file = userDetail.ImageUrl;

                    if (file == null || file.Length == 0)
                    {
                        return BadRequest("No file uploaded.");
                    }

                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    using (var stream = file.OpenReadStream())
                    {
                        var task = new FirebaseStorage("court-callers.appspot.com")
                            .Child("NewsImages")
                            .Child(fileName)
                            .PutAsync(stream);

                        var downloadUrl = await task;
                        userDetail.ProfilePicture = downloadUrl; // Directly assign the URL
                    }
                }

                await _userDetailService.UpdateUserDetail(userDetail, id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error adding product: {ex.Message}");
            }
            //try
            //{
            //    await _userDetailService.UpdateUserDetail(userDetail, id);
            //    return Ok();
            //}
            //catch (Exception ex)
            //{
            //    return StatusCode(500, $"Error updating user detail: {ex.Message}");
            //}
        }
    }
}
