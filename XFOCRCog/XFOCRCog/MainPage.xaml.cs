using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XFOCRCog {
	public partial class MainPage : ContentPage {
		public MainPage() {
			InitializeComponent();
			capture.Clicked += Capture_Clicked;
		}

		private async void Capture_Clicked(object sender, EventArgs e) {
			await CrossMedia.Current.Initialize();

			MediaFile photo;
			if (CrossMedia.Current.IsCameraAvailable) {
				photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions {
					Directory = "bcard",
					Name = "bcard.jpg"
				});
			}
			else {
				photo = await CrossMedia.Current.PickPhotoAsync();
			}

			if (photo == null) return;

			OcrResults text;
			System.Diagnostics.Debug.WriteLine("clog@" + DateTime.Now.ToString("mm:ss.fff") + "	:	about to OCR");
			var client = new VisionServiceClient(""); // cognitive api key - https://www.microsoft.com/cognitive-services/en-US/subscriptions
			using (var photoStream = photo.GetStream()) {
				text = await client.RecognizeTextAsync(photoStream);
			}

			System.Diagnostics.Debug.WriteLine("clog@" + DateTime.Now.ToString("mm:ss.fff") + "	:	done OCRing");
			var builder = new StringBuilder();

			var rcount = 0;
			var lcount = 0;
			foreach (var region in text.Regions) {
				builder.AppendLine($"region {rcount}");
				foreach (var line in region.Lines) {
					builder.AppendLine($"line {lcount}");
					foreach (var word in line.Words) {
						builder.Append(word.Text + " ");
					}
					builder.AppendLine();
					lcount++;
				}
				rcount++;
			}

			label.Text = builder.ToString();
		}
	}
}
