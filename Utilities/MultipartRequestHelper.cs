using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using StartupsBack.JsonConverters;
using StartupsBack.Models.DbModels;
using StartupsBack.ViewModels;
using StartupsBack.ViewModels.ActionsResults;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace StartupsBack.Utilities
{
    public static class MultipartRequestHelper
    {
        // Content-Type: multipart/form-data; boundary="----WebKitFormBoundarymx2fSWqWSd0OxQqq"
        // The spec at https://tools.ietf.org/html/rfc2046#section-5.1 states that 70 characters is a reasonable limit.
        public static string GetBoundary(MediaTypeHeaderValue contentType, long lengthLimit = int.MaxValue)
        {
            var boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary).Value;

            if (string.IsNullOrWhiteSpace(boundary))
            {
                throw new InvalidDataException("Missing content-type boundary.");
            }

            if (boundary.Length > lengthLimit)
            {
                throw new InvalidDataException(
                    $"Multipart boundary length limit {lengthLimit} exceeded.");
            }

            return boundary;
        }

        public static async Task<UserParseResult> GetUserModelFromMultipart(HttpRequest httpRequest, ModelStateDictionary modelState,
            string[] permittedExtensions, long fileSizeLimit)
        {
            try
            {
                if (!IsMultipartContentType(httpRequest.ContentType))
                {
                    modelState.AddModelError("File",
                        $"The request couldn't be processed (Error 1).");

                    return UserParseResult.NotMultipart();
                }

                var userModel = new UserModel();

                var boundary = GetBoundary(
                    MediaTypeHeaderValue.Parse(httpRequest.ContentType), fileSizeLimit);

                var reader = new MultipartReader(boundary, httpRequest.Body);
                var section = await reader.ReadNextSectionAsync();

                while (section != null)
                {
                    var hasContentDispositionHeader =
                        ContentDispositionHeaderValue.TryParse(
                            section.ContentDisposition, out var contentDisposition);

                    if (hasContentDispositionHeader)
                    {
                        // This check assumes that there's a file
                        // present without form data. If form data
                        // is present, this method immediately fails
                        // and returns the model error.
                        if (!HasFileContentDisposition(contentDisposition))
                        {
                            if (contentDisposition?.Name == JsonConstants.UserName)
                            {
                                var val = await section.ReadAsStringAsync();
                                userModel.Name = val;
                            }
                            else if (contentDisposition?.Name == JsonConstants.UserPassword)
                            {
                                var val = await section.ReadAsStringAsync();
                                userModel.PasswordHash = val;
                            }
                            else
                            {
                                /*modelState.AddModelError("File",
                                 $"The request couldn't be processed (Error 2).");*/
                            }
                        }
                        else
                        {
                            var streamedFileContent = await FileHelpers.ProcessStreamedFile(
                                section, contentDisposition, modelState,
                                permittedExtensions, fileSizeLimit);

                            userModel.ProfilePic = streamedFileContent;
                        }
                    }

                    // Drain any remaining section body that hasn't been consumed and
                    // read the headers for the next section.
                    section = await reader.ReadNextSectionAsync();
                }

                if (!modelState.IsValid)
                {
                    return UserParseResult.BadModel();
                }

                return UserParseResult.Success(userModel);
            }
            catch (Exception ex)
            {
                return UserParseResult.UnknownError(ex);
            }
        }
        public static Task<MultipartFormDataContent> UserModelToMultipart(string formDataBoundary, UserModel userModel, bool needPic, bool myProfile)
        {
            var formData = new MultipartFormDataContent(formDataBoundary);

            if (myProfile)
                formData.Add(new StringContent(userModel.Token), JsonConstants.UserToken);

            formData.Add(new StringContent(userModel.Name), JsonConstants.UserName);

            if (needPic)
            {
                var file_bytes = userModel.ProfilePic;
                formData.Add(new ByteArrayContent(file_bytes, 0, file_bytes.Length), JsonConstants.UserPicturePropertyName, JsonConstants.UserPictureFileName);
            }

            return Task.FromResult(formData);
        }

        public static bool IsMultipartContentType(string? contentType)
        {
            return !string.IsNullOrEmpty(contentType)
                   && contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static bool HasFormDataContentDisposition(ContentDispositionHeaderValue? contentDisposition)
        {
            // Content-Disposition: form-data; name="key";
            return contentDisposition != null
                && contentDisposition.DispositionType.Equals("form-data")
                && string.IsNullOrEmpty(contentDisposition.FileName.Value)
                && string.IsNullOrEmpty(contentDisposition.FileNameStar.Value);
        }

        public static bool HasFileContentDisposition(ContentDispositionHeaderValue? contentDisposition)
        {
            // Content-Disposition: form-data; name="myfile1"; filename="Misc 002.jpg"
            return contentDisposition != null
                && contentDisposition.DispositionType.Equals("form-data")
                && (!string.IsNullOrEmpty(contentDisposition.FileName.Value)
                    || !string.IsNullOrEmpty(contentDisposition.FileNameStar.Value));
        }
    }
}
