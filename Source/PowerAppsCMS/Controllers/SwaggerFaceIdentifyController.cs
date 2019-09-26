using System;
using System.Net;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System.Threading.Tasks;

namespace PowerAppsCMS.Controllers
{
    /// <summary>
    /// Controller yang digunakan untuk Face Identify
    /// </summary>
    public class SwaggerFaceIdentifyController : SwaggerFaceAPIController
    {
        /// <summary>
        /// Mengidentifikasi apakah user yang login memiliki akun di face api,
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="faceID"></param>
        /// <returns>PersonID user</returns>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "Identify face",
            Type = typeof(Person))]
        [SwaggerResponse(HttpStatusCode.InternalServerError,
                Description = "Internal Server Error",
               Type = typeof(Exception))]
        [SwaggerOperation("Identify")]

        public async Task<IHttpActionResult> identifyFace(string groupId, string faceID)
        {
            Person personResult = null;
            //var personName = string.Empty;
            Guid[] guidFaceID = { Guid.Parse(faceID.Replace(" ", string.Empty)) };
            try
            {
                foreach (IdentifyResult tryIdentifyResult in await faceServiceClient.IdentifyAsync(groupId.ToLower(), guidFaceID))
                {
                    if (tryIdentifyResult.Candidates.Length != 0)
                    {
                        if (tryIdentifyResult.Candidates[0].Confidence > 0.8)
                        {
                            var candidateId = tryIdentifyResult.Candidates[0].PersonId;

                            var person = await faceServiceClient.GetPersonAsync(groupId, candidateId);
                            //personName = person.Name;
                            personResult = person;
                        }
                    }
                }
                //IdentifyResult[] identityResult = await faceServiceClient.IdentifyAsync(groupId, faceId.ToArray());
                return Ok(personResult);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
