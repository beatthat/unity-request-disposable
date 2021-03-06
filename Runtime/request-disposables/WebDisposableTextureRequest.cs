using UnityEngine;
using System;
using UnityEngine.Networking;
using BeatThat.Disposables;

namespace BeatThat.Requests
{
	/// <summary>
	/// TODO: change so that the request can be disposed immediately and the loaded clip disposed elsewhere/later
	/// </summary>
	public class WebDisposableTextureRequest : WebRequest, Request<Disposable<Texture2D>>
	{
		//public WebDisposableTextureRequest(WebRequestRunner runner, string url = null, HttpVerb verb = HttpVerb.GET, float delay = 0f) 
			//: base(runner, url, verb, delay) {}

        public WebDisposableTextureRequest(string url = null, HttpVerb verb = HttpVerb.GET, float delay = 0f)
            : base(url, verb, delay) { }



		public object GetItem() { return this.item; } 
		public Disposable<Texture2D> item { get; protected set; }

		override protected UnityWebRequest PrepareRequest() 
		{
			var req = base.PrepareRequest();
			this.downloadHandler = new DownloadHandlerTexture();
			req.downloadHandler = this.downloadHandler;
			return req;
		}

		virtual public void Execute(Action<Request<Disposable<Texture2D>>> callback) 
		{
			if(callback == null) {
				Execute();
				return;
			}

			RequestExecutionPool<Disposable<Texture2D>>.Get().Execute(this, callback);
		}

//		override protected void DoOnError()
//		{
//			if(this.www == null) {
//				return;
//			}
//
//			var err = this.www.error;
//			
//			Debug.LogError("Failed to retrieve item from url '" + this.www.url + "', error='" + err
//			               + "', req.error='" + this.www.error + "'");
//			
//			this.error = err;
//		}

		override protected void DoOnDone()
		{
			this.item = new DownloadedAssetDisposable<Texture2D>((this.www.downloadHandler as DownloadHandlerTexture).texture);
		}

		override protected void AfterDisposeWWW()
		{
			var d = this.downloadHandler;
			this.downloadHandler = null;
//			if(d != null) {
//				StaticObjectPool<DownloadHandlerTexture>.Return(d);
//			}
		}

		private DownloadHandlerTexture downloadHandler { get; set; }
	}
}