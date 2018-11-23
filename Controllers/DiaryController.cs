using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DearDiaryLogs.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using MemeBank.Helpers;

namespace DearDiaryLogs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiaryController : ControllerBase
    {
        private readonly DearDiaryLogsContext _context;
        private readonly IConfiguration _configuration;

        public DiaryController(DearDiaryLogsContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/Diary
        [HttpGet]
        public IEnumerable<DiaryLog> GetDiaryLog()
        {
            return _context.DiaryLog.Include("Images");
        }

        // GET: api/Diary/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDiaryLog([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //var diaryLog = await _context.Users.Include("DiaryLogs").Where(s => s.Id == id).ToListAsync();
            var diaryLog = await _context.DiaryLog.Include("Images").Where(s => s.UserId == id).ToListAsync();
            if (diaryLog == null)
            {
                return NotFound();
            }

            return Ok(diaryLog);
        }

        [Route("SearchByEventName/{Event}")]
        [HttpGet]
        public async Task<IActionResult> GetLogByEvent([FromRoute] String Event)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var diaryLog = await _context.DiaryLog.Where(s => s.EventName == Event).ToListAsync();

            if (diaryLog == null)
            {
                return NotFound();
            }

            return Ok(diaryLog);
        }

        [Route("distinctDates")]
        [HttpGet]
        public async Task<List<string>> GetDistinctDates()
        {


            var diaryLog = await _context.DiaryLog.Select(s => s.StartTime).Distinct().ToListAsync();

            return diaryLog;
        }

        // PUT: api/Diary/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDiaryLog([FromRoute] int id, [FromBody] DiaryLog diaryLog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != diaryLog.Id)
            {
                return BadRequest();
            }

            _context.Entry(diaryLog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DiaryLogExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Diary
        [HttpPost]
        public async Task<IActionResult> PostDiaryLog([FromBody] DiaryLog diaryLog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.DiaryLog.Add(diaryLog);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDiaryLog", new { id = diaryLog.Id }, diaryLog);
        }

        [HttpPost, Route("CreateUser")]
        public async Task<IActionResult> CreateUser([FromForm] UserEntry userEntry)
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }
            else
            {
                try
                {
                    // To create a user, create a User from the UserEntry
                    // Add and save the changes

                    Users newUser = new Users
                    {
                        Username = userEntry.Username,
                        Password = userEntry.Password
                    };

                    _context.Add(newUser);
                    await _context.SaveChangesAsync();

                    return Ok(newUser.Id);
                }
                catch(Exception e)
                {
                    return BadRequest($"An error has occured g: {e.Message}");
                }
            }
        }

        [HttpPost, Route("Upload")]
        public async Task<IActionResult> UploadAndPost([FromForm] DiaryEntry diaryEntry)
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }
            else
            {
                try
                {
                    // A diaryEntry could have multiple files or None.

                    // We have to store the URI of each file in the DiaryImage table with its , associated with a eventID.
                    // Save the event ID in a variable.

                    // Create a diary log and upload the details.
                    // Make the Story URI
                    /* if (not zero files)
                     *   there is files
                     *   go through each file
                     *   upload the file to a blob and get the URI
                     *   Put the URI along with the width/height in a new diary image instance with the eventID created
                     *   Save changes 
                    */

                    // CloudBlockBlob uploadedTextBlob = await UploadTextToBlob(diaryEntry.Story);
                    int eventID;
                    // Successfully uploaded Blob and identifier is found
                    DiaryLog diaryLogInstance = new DiaryLog
                    {
                        UserId = diaryEntry.UserID,
                        EventName = diaryEntry.Event,
                        StoryUrl = diaryEntry.Story,
                        StartTime = diaryEntry.StartTime,
                        EndTime = diaryEntry.EndTime
                    };


                    _context.DiaryLog.Add(diaryLogInstance);

                    await _context.SaveChangesAsync();
                    eventID = diaryLogInstance.Id;
                    // if 0 images, nothing will be attempted to upload
                    if (diaryEntry.Images != null && diaryEntry.Images.Any())
                    {

                        foreach (IFormFile imageInstance in diaryEntry.Images)
                        {
                            // Read images as a stream of data so it can be uploaded to blob

                            using (System.IO.Stream stream = imageInstance.OpenReadStream())
                            {
                                // Create and upload a blob for the image.

                                CloudBlockBlob uploadedImageBlob = await UploadImageToBlob(imageInstance.FileName, null, stream);

                                string uploadedImageBlobURI = uploadedImageBlob.StorageUri.ToString();

                                if (!string.IsNullOrEmpty(uploadedImageBlobURI))
                                {
                                    System.Drawing.Image uploadedImage = System.Drawing.Image.FromStream(stream);
                                    // If there is a valid URI, then upload
                                    DiaryImage diaryImageInstance = new DiaryImage
                                    {
                                        // Problem : This is trying to upload before primary key eventId is made
                                        EntryId = eventID,
                                        ImageURL = uploadedImageBlob.SnapshotQualifiedUri.AbsoluteUri,
                                        Height = uploadedImage.Height.ToString(),
                                        Width = uploadedImage.Width.ToString()
                                    };
                                    // Add this new instance to the dbset (collection of entities) in memory
                                    _context.DiaryImage.Add(diaryImageInstance);


                                }
                            }
                        }
                    }

                    // Save all the changes to the database
                    await _context.SaveChangesAsync();

                    return Ok($"The file {diaryEntry.Event} has been succesfully uploaded");
                    
                    #region
                    /*
                    using (System.IO.Stream stream = diaryEntry.Image.OpenReadStream())
                    {
                        // create and upload Blob for Image and Story
                        CloudBlockBlob uploadedImageBlob = await UploadImageToBlob(diaryEntry.Image.FileName, null, stream);
                        CloudBlockBlob uploadedTextBlob = await UploadTextToBlob(diaryEntry.Story);

                        // The URI is an identifier for the blob. Its a more general version of a url.
                        string uploadedImageBlobURI = uploadedImageBlob.StorageUri.ToString();
                        string uploadedTextBlobURI = uploadedTextBlob.StorageUri.ToString();

                        // If the URI is empty (there is no identifier for the blob), then something went wrong
                        if (string.IsNullOrEmpty(uploadedImageBlobURI) || string.IsNullOrEmpty(uploadedTextBlobURI))
                        {
                            return BadRequest("Error when uploading: Identifier not found. Please Try Again");
                        }
                        else
                        {
                            // Can now begin uploading data to database

                            System.Drawing.Image image = System.Drawing.Image.FromStream(stream);

                            // Create an instance of a Diary Log
                            DiaryLog diaryLog = new DiaryLog
                            {
                                EventName = diaryEntry.Event,
                                StoryUrl = uploadedTextBlob.SnapshotQualifiedUri.AbsoluteUri, // Get the URI for the text blob
                                StartTime = diaryEntry.StartTime,
                                EndTime = diaryEntry.EndTime,
                                ImageUrl = uploadedImageBlob.SnapshotQualifiedUri.AbsoluteUri, // Get the URI for the image blob
                                Height = image.Height.ToString(),
                                Width = image.Width.ToString()
                            };

                            // Adding the files to the database
                            _context.DiaryLog.Add(diaryLog);
                            await _context.SaveChangesAsync();

                            return Ok($"File: {diaryEntry.Event} has been successfully uploaded to the database");
                        }
                    }
                    */
                    #endregion
                }
                catch(Exception e)
                {
                    return BadRequest($"An error has occured g: {e.Message}");
                }
            }
        }

        private async Task<CloudBlockBlob> UploadImageToBlob(string fileName, byte[] imageBuffer = null, System.IO.Stream stream = null)
        {
            // A function that uploads a file to a blob given a file name

            
            string accountName = _configuration["AzureBlob:name"];
            string accountKey = _configuration["AzureBlob:key"];
            string connectionString = _configuration["AzureBlob:connectionString"];

            // Storing the credentials
            StorageCredentials credentials = new StorageCredentials(accountName, accountKey);

            // Creating an instance of an account
            CloudStorageAccount storageAccount = new CloudStorageAccount(credentials, true);

            // Instantiating a CloudBlobClient
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Get a reference to the imageContainer that stores the blobs
            CloudBlobContainer blobContainer = blobClient.GetContainerReference("images");

            // Attempts to parse (convert string to its equivalent storage Account)
            if (CloudStorageAccount.TryParse(connectionString, out storageAccount))
            {
                
                try
                {
                    // Get a unique ID using NewGuid(), then add the file extension.
                    string extension = fileName.Contains(".") ? "." + fileName.Split('.').Last() : "";
                    string blobName = Guid.NewGuid().ToString() + extension;
                    

                    // Create an instance of a Blob represented by blocks in the container
                    // Block blobs are ideal for storing text
                    CloudBlockBlob cloudBlockBlob = blobContainer.GetBlockBlobReference(blobName);

                    // If there is an image available (stream), then begin the upload.
                    if(stream != null)
                    {
                        await cloudBlockBlob.UploadFromStreamAsync(stream);
                    }
                    else
                    {
                        // return an empty URI
                        return new CloudBlockBlob(new Uri(""));
                    }
                    // If successfull, return the Blob uploaded
                    return cloudBlockBlob;
                }
                catch
                {
                    // return an empty URI
                    return new CloudBlockBlob(new Uri(""));
                }
            }
            else
            {
                // return an empty URI
                return new CloudBlockBlob(new Uri(""));
            }
        }

        private async Task<CloudBlockBlob> UploadTextToBlob(string text)
        {
            string accountName = _configuration["AzureBlob:name"];
            string accountKey = _configuration["AzureBlob:key"];
            string connectionString = _configuration["AzureBlob:connectionString"];

            StorageCredentials credentials = new StorageCredentials(accountName, accountKey);

            CloudStorageAccount storageAccount = new CloudStorageAccount(credentials, true);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer blobContainer = blobClient.GetContainerReference("stories");

            // Even empty strings can be uploaded and edited later.
            try
            {
                CloudBlockBlob Blob = blobContainer.GetBlockBlobReference(Guid.NewGuid().ToString() + ".txt");
                await Blob.UploadTextAsync(text);
                return Blob;
            }
            catch
            {
                // return an empty URI
                return new CloudBlockBlob(new Uri(""));
            }



        }

        // DELETE: api/Diary/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiaryLog([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var diaryLog = await _context.DiaryLog.FindAsync(id);
            if (diaryLog == null)
            {
                return NotFound();
            }

            // By removing the diaryLog's, the images are also removed via the navigation property
            _context.DiaryLog.Remove(diaryLog);
            await _context.SaveChangesAsync();

            return Ok(diaryLog);
        }

        private bool DiaryLogExists(int id)
        {
            return _context.DiaryLog.Any(e => e.Id == id);
        }
    }
}