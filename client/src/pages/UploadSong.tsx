import { useState } from "react";
import useMusicCrud from "../useMusicCrud";

export default function UploadSong() {
    const { uploadSong } = useMusicCrud();

    const [file, setFile] = useState<File | null>(null);
    const [title, setTitle] = useState("");
    const [isUploading, setIsUploading] = useState(false);

    const handleUpload = async () => {
        if (!file) {
            alert("Please select a song file.");
            return;
        }

        try {
            setIsUploading(true);
            await uploadSong(file, title);
            alert("Song uploaded successfully!");
            setFile(null);
            setTitle("");
        } catch (error) {
            alert("Upload failed.");
        } finally {
            setIsUploading(false);
        }
    };

    return (
        <div className="h-screen flex flex-col overflow-hidden bg-base-200">
            <header className="bg-base-100 shadow-sm border-b border-base-300 flex-shrink-0">
                <div className="navbar px-4 md:px-8 py-3">
                    <div className="flex-1">
                        <span className="text-xl font-bold text-primary ml-2 md:ml-0">
                            Music App
                        </span>
                    </div>
                </div>
            </header>

            <main className="flex-1 flex items-center justify-center p-6">
                <div className="card w-full max-w-lg bg-base-100 shadow-xl">
                    <div className="card-body">
                        <h2 className="card-title text-primary">
                            Upload New Song
                        </h2>

                        <div className="form-control w-full mt-2">
                            <label className="label">
                                <span className="label-text">Song Title</span>
                            </label>
                            <input
                                type="text"
                                placeholder="Enter song title"
                                className="input input-bordered w-full"
                                value={title}
                                onChange={(e) => setTitle(e.target.value)}
                            />
                        </div>

                        <div className="form-control w-full mt-4">
                            <label className="label">
                                <span className="label-text">Select MP3 File</span>
                            </label>
                            <input
                                type="file"
                                accept="audio/mpeg"
                                className="file-input file-input-bordered w-full"
                                onChange={(e) =>
                                    setFile(e.target.files ? e.target.files[0] : null)
                                }
                            />
                        </div>

                        <div className="card-actions justify-end mt-6">
                            <button
                                className={`btn btn-primary ${isUploading ? "loading" : ""}`}
                                onClick={handleUpload}
                                disabled={isUploading}
                            >
                                {isUploading ? "Uploading..." : "Upload Song"}
                            </button>
                        </div>
                    </div>
                </div>
            </main>
        </div>
    );
}