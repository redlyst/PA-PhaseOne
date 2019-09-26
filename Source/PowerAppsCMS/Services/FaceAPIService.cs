using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using PowerAppsCMS.Models;
using System.Configuration;

namespace PowerAppsCMS.Services
{
    /// <summary>
    /// Kumpulan service untuk 
    /// </summary>
    public class FaceAPIService
    {
        private readonly string faceAPIKey;
        private readonly string faceAPIEndPoint;
        private readonly FaceServiceClient faceServiceClient;
        private readonly string PERSON_GROUP_ID = ConfigurationManager.AppSettings["PersonGroupID"];
        private readonly string PERSON_GROUP_NAME = ConfigurationManager.AppSettings["PersonGroupName"];

        public FaceAPIService(string faceAPIKey, string faceAPIEndPoint)
        {
            this.faceAPIKey = faceAPIKey;
            this.faceAPIEndPoint = faceAPIEndPoint;
            faceServiceClient = new FaceServiceClient(faceAPIKey, faceAPIEndPoint);
        }

        // Returns true if face matches.
        public async Task<bool> AuthenticateFaceAsync(string imageUrl, Guid personId)
        {
            var faces = await faceServiceClient.DetectAsync(imageUrl);
            var faceIds = faces.Select(face => face.FaceId).ToArray();

            var result = (await faceServiceClient.IdentifyAsync(PERSON_GROUP_ID, faceIds, 3));
            var identifyResults = result.OfType<Microsoft.ProjectOxford.Face.Contract.IdentifyResult>().ToList();

            foreach (var identifyResult in identifyResults)
            {
                if (identifyResult.Candidates.Length == 0)
                    continue;

                var candidateId = identifyResult.Candidates[0].PersonId;
                var person = await faceServiceClient.GetPersonAsync(PERSON_GROUP_ID, candidateId);

                if (person.PersonId == personId)
                {
                    return true;
                }
            }

            return false;
        }

        // Returns person ID.
        public async Task<Guid> CreatePersonAsync(string pegawaiId, string name)
        {
            await CreatePersonGroupIfNotExist();

            var result = await faceServiceClient.CreatePersonAsync(PERSON_GROUP_ID, $"{name}");
            return result.PersonId;
        }

        // Adds new person picture.
        public async Task AddPersonPicture(List<string> images, Guid personId)
        {
            await CreatePersonGroupIfNotExist();
            foreach (var image in images)
            {
                var addPersistedFaceResult = await faceServiceClient.AddPersonFaceInPersonGroupAsync(PERSON_GROUP_ID, personId, image);
            }

            await TrainModelAsync();
        }

        // Deletes previous person picture.
        public async Task DeletePersonPicture(string imageUrl, Guid personId)
        {
            await CreatePersonGroupIfNotExist();

            var faces = await faceServiceClient.DetectAsync(imageUrl);
            var faceIds = faces.Select(face => face.FaceId).ToArray();

            foreach (var faceId in faceIds)
            {
                await faceServiceClient.DeletePersonFaceFromPersonGroupAsync(PERSON_GROUP_ID, personId, faceId);
            }

            await TrainModelAsync();
        }

        // Model is retrained every time new person picture is added or deleted (see AddPersonPicture and DeletePersonPicture
        private async Task TrainModelAsync()
        {
            await faceServiceClient.TrainPersonGroupAsync(PERSON_GROUP_ID);

            Microsoft.ProjectOxford.Face.Contract.TrainingStatus trainingStatus = null;
            while (true)
            {
                trainingStatus = await faceServiceClient.GetPersonGroupTrainingStatusAsync(PERSON_GROUP_ID);

                if (trainingStatus.Status != Status.Running)
                {
                    break;
                }

                await Task.Delay(1000);
            }
        }

        private async Task CreatePersonGroupIfNotExist()
        {
            Microsoft.ProjectOxford.Face.Contract.PersonGroup personGroup;

            try
            {
                personGroup = await faceServiceClient.GetPersonGroupAsync(PERSON_GROUP_ID);
            }
            catch (FaceAPIException)
            {
                await faceServiceClient.CreatePersonGroupAsync(PERSON_GROUP_ID, PERSON_GROUP_NAME);
                personGroup = await faceServiceClient.GetPersonGroupAsync(PERSON_GROUP_ID);
            }
        }

        public async Task DeletePerson(Guid ID)
        {
            PowerAppsCMSEntities db = new PowerAppsCMSEntities();
            var getPerson = db.Users.Find(ID);

            Guid guidPersonID = new Guid(getPerson.PersonID.Replace(" ", string.Empty));

            await faceServiceClient.DeletePersonAsync(PERSON_GROUP_ID, guidPersonID);
        }
    }
}
