using System.IO;
using Android.Graphics;

namespace BookSearch
{
    public static class BitmapHelpers
    {
        public static Bitmap LoadAndResizeBitmap(this string fileName, int width, int height)
        {
            // получаем размер файла на диске
            BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
            BitmapFactory.DecodeFile(fileName, options);

            // Вычислить соотношение, которое необходимо изменить чтобы соответствовало запрошенному
            int outHeight = options.OutHeight;
            int outWidth = options.OutWidth;
            int inSampleSize = 1;

            if (outHeight > height || outWidth > width)
            {
                inSampleSize = outWidth > outHeight
                                   ? outHeight / height
                                   : outWidth / width;
            }

            // Загружаем изображение и изменяем его размер для BitmapFactory 
            options.InSampleSize = inSampleSize;
            options.InJustDecodeBounds = false;
            Bitmap resizedBitmap = BitmapFactory.DecodeFile(fileName, options);

            return resizedBitmap;
        }

        public static Stream BitmapToStream(Bitmap bitmap)
        {
            byte[] data;
            using (var stream = new MemoryStream())
            {
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                data = stream.ToArray();
            }
            return new MemoryStream(data);
            
        }
    }
}