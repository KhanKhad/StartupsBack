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
using System.Linq;
using System.Net.Http;
using System.Text;
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
                            userModel.ProfilePicFileName = contentDisposition?.FileName.Value ?? string.Empty;
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
        public static Task<MultipartFormDataContent> UserModelToMultipart(string formDataBoundary, UserModel? userModel, bool isMine, bool needFull)
        {
            if (userModel == null) throw new NullReferenceException(nameof(userModel));

            var formData = new MultipartFormDataContent(formDataBoundary);

            if (isMine)
                formData.Add(new StringContent(userModel.Token), JsonConstants.UserToken);

            formData.Add(new StringContent(userModel.Name), JsonConstants.UserName);

            if (needFull)
            {
                var file_bytes = userModel.ProfilePic;
                formData.Add(new ByteArrayContent(file_bytes, 0, file_bytes.Length), JsonConstants.UserPicturePropertyName, userModel.ProfilePicFileName);
            }

            return Task.FromResult(formData);
        }

        public static async Task<StartupParseResult> GetStartupModelFromMultipart(HttpRequest httpRequest, ModelStateDictionary modelState,
            string[] permittedExtensions, long fileSizeLimit)
        {
            try
            {
                if (!IsMultipartContentType(httpRequest.ContentType))
                {
                    modelState.AddModelError("File",
                        $"The request couldn't be processed (Error 1).");

                    return StartupParseResult.NotMultipart();
                }

                var startupModel = new StartupModel();
                var authorName = string.Empty;
                var startupHash = string.Empty;
                

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
                            if (contentDisposition?.Name == JsonConstants.StartupName)
                            {
                                var val = await section.ReadAsStringAsync();
                                startupModel.Name = val;
                            }
                            else if (contentDisposition?.Name == JsonConstants.StartupDescription)
                            {
                                var val = await section.ReadAsStringAsync();
                                startupModel.Description = val;
                            }
                            else if (contentDisposition?.Name == JsonConstants.StartupAuthorName)
                            {
                                var val = await section.ReadAsStringAsync();
                                authorName = val;
                            }
                            else if (contentDisposition?.Name == JsonConstants.StartupHash)
                            {
                                var val = await section.ReadAsStringAsync();
                                startupHash = val;
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
                            startupModel.StartupPicFileName = contentDisposition?.FileName.Value ?? string.Empty;
                            startupModel.Picture = streamedFileContent;
                        }
                    }

                    // Drain any remaining section body that hasn't been consumed and
                    // read the headers for the next section.
                    section = await reader.ReadNextSectionAsync();
                }

                if (!modelState.IsValid)
                {
                    return StartupParseResult.BadModel();
                }

                return StartupParseResult.Success(startupModel, authorName, startupHash);
            }
            catch (Exception ex)
            {
                return StartupParseResult.UnknownError(ex);
            }
        }

        public static Task<MultipartFormDataContent> StartupModelToMultipart(string formDataBoundary, StartupModel? startupModel, bool isMine, bool needFull)
        {
            if(startupModel == null) throw new NullReferenceException(nameof(startupModel));

            var formData = new MultipartFormDataContent(formDataBoundary);

            formData.Add(new StringContent(startupModel.Id.ToString()), JsonConstants.StartupId);
            formData.Add(new StringContent(startupModel.Name), JsonConstants.StartupName);
            formData.Add(new StringContent(startupModel.Description), JsonConstants.StartupDescription);
            
            if(needFull)
            {
                var sb = new StringBuilder();
                sb.AppendJoin(',', startupModel.Contributors.Select(i => i.Id));
                formData.Add(new StringContent(sb.ToString()), JsonConstants.StartupContributorsIds);
            }

            var file_bytes = startupModel.Picture;
            formData.Add(new ByteArrayContent(file_bytes, 0, file_bytes.Length), JsonConstants.StartupPicturePropertyName, startupModel.StartupPicFileName);

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
