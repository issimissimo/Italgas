using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public class FileUploader
{
	public enum STATE { SUCCESS, ERROR }
	public class Result
	{
		public STATE state;
	}



	public IEnumerator UploadToFTPCoroutine(string fileFullPath, string newName, string server, string username, string password, string initialPath, Action<Result> callback = null)
	{
		Result result = new Result();

		var file = new FileInfo(fileFullPath);
		var newFileName = String.IsNullOrEmpty(newName) ? file.Name : newName;
		var address = new Uri("ftp://" + server + "/" + Path.Combine(initialPath, newFileName));

		Debug.Log("TRY TO UPLOAD TO: " + address);

		// var address = new Uri(Path.Combine(serverPath, file.Name));
		var request = FtpWebRequest.Create(address) as FtpWebRequest;

		// Upload options:

		// Provide credentials
		request.Credentials = new NetworkCredential(username, password);

		// Set control connection to closed after command execution
		request.KeepAlive = false;

		// Specify command to be executed
		request.Method = WebRequestMethods.Ftp.UploadFile;

		// Specify data transfer type
		request.UseBinary = true;

		// Notify server about size of uploaded file
		request.ContentLength = file.Length;

		// Set buffer size to 2KB.
		var bufferLength = 2048;
		var buffer = new byte[bufferLength];
		var contentLength = 0;

		// Open file stream to read file
		var fs = file.OpenRead();
		var stream = request.GetRequestStream();

		contentLength = fs.Read(buffer, 0, bufferLength);

		// Loop until stream content ends.
		while (contentLength != 0)
		{
			// Debug.Log("Progress: " + ((fs.Position / fs.Length) * 100f));
			// Write content from file stream to FTP upload stream.
			stream.Write(buffer, 0, contentLength);
			contentLength = fs.Read(buffer, 0, bufferLength);
			yield return null;
		}

		stream.Close();
		fs.Close();

		result.state = STATE.SUCCESS;

		if(callback != null) callback(result);
	}




	public IEnumerator UploadMultipleFilesToFTPCoroutine(List <string> fileFullPathList, string server, string username, string password, string initialPath, Action<Result> callback = null)
	{
		Result result = new Result();

		var file = new FileInfo(fileFullPathList[0]);
		var address = new Uri("ftp://" + server + "/" + Path.Combine(initialPath, file.Name));

		Debug.Log("TRY TO UPLOAD TO: " + address);

		// var address = new Uri(Path.Combine(serverPath, file.Name));
		var request = FtpWebRequest.Create(address) as FtpWebRequest;

		// Upload options:

		// Provide credentials
		request.Credentials = new NetworkCredential(username, password);

		// Set control connection to closed after command execution
		request.KeepAlive = false;

		// Specify command to be executed
		request.Method = WebRequestMethods.Ftp.UploadFile;

		// Specify data transfer type
		request.UseBinary = true;

		// Notify server about size of uploaded file
		request.ContentLength = file.Length;

		// Set buffer size to 2KB.
		var bufferLength = 2048;
		var buffer = new byte[bufferLength];
		var contentLength = 0;

		// Open file stream to read file
		var fs = file.OpenRead();
		var stream = request.GetRequestStream();

		contentLength = fs.Read(buffer, 0, bufferLength);

		// Loop until stream content ends.
		while (contentLength != 0)
		{
			// Debug.Log("Progress: " + ((fs.Position / fs.Length) * 100f));
			// Write content from file stream to FTP upload stream.
			stream.Write(buffer, 0, contentLength);
			contentLength = fs.Read(buffer, 0, bufferLength);
			yield return null;
		}

		stream.Close();
		fs.Close();

		result.state = STATE.SUCCESS;

		if(callback != null) callback(result);
	}
}
