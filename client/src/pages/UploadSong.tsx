import { useState } from "react";
import { useAtomValue } from "jotai";
import { userAtom } from "../atoms/userAtom";
import { useNavigate } from "react-router-dom";
import useMusicCrud from "../useMusicCrud";

export default function UploadSong() {
    const user = useAtomValue(userAtom);
    const navigate = useNavigate();
    const { logout, uploadSong } = useMusicCrud();

    const username = user?.username ?? "Guest";

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
            await uploadSong(file);
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

            <div className="flex-1 flex overflow-hidden">
                <aside className="bg-base-100 border-r border-base-300 w-72 flex-shrink-0 flex flex-col h-full">
                    <div className="flex items-center gap-3 p-4">
                        <div className="w-10 h-10 rounded-full overflow-hidden border-2 border-primary">
                            <img
                                src={`https://api.dicebear.com/7.x/avataaars/svg?seed=${username}`}
                                alt="User avatar"
                                className="w-full h-full"
                            />
                        </div>
                        <span className="font-medium text-base-content">
                            {username}
                        </span>
                    </div>

                    {!user && (
                        <button
                            className="btn btn-primary btn-sm mt-2 ml-10 w-50"
                            onClick={() => navigate("/login")}
                        >
                            Login
                        </button>
                    )}

                    <div className="flex flex-col justify-between flex-1 p-6">
                        <nav className="flex flex-col justify-center space-y-2 flex-1">
                            <a className="flex items-center gap-3 p-3 rounded-lg hover:bg-base-200 transition-colors">
                                <span className="text-xl">🏠</span>
                                <span onClick={() => navigate("/home")}>
                                    Home
                                </span>
                            </a>
                            <a className="flex items-center gap-3 p-3 rounded-lg bg-primary/10 text-primary font-medium">
                                <span className="text-xl">⬆️</span>
                                <span>Upload Song</span>
                            </a>
                        </nav>

                        {user && (
                            <div className="flex justify-center mt-4">
                                <button
                                    className="btn btn-primary w-60"
                                    onClick={() => logout()}
                                >
                                    Logout
                                </button>
                            </div>
                        )}
                    </div>
                </aside>

                <main className="flex-1 flex items-center justify-center p-6">
                    <div className="card w-full max-w-lg bg-base-100 shadow-xl">
                        <div className="card-body">
                            <h2 className="card-title text-primary">
                                Upload New Song
                            </h2>

                            <div className="form-control w-full">
                                <label className="label">
                                    <span className="label-text">
                                        Song Title
                                    </span>
                                </label>
                                <input
                                    type="text"
                                    placeholder="Enter song title"
                                    className="input input-bordered w-full"
                                    value={title}
                                    onChange={(e) =>
                                        setTitle(e.target.value)
                                    }
                                />
                            </div>

                            <div className="form-control w-full mt-4">
                                <label className="label">
                                    <span className="label-text">
                                        Select MP3 File
                                    </span>
                                </label>
                                <input
                                    type="file"
                                    accept="audio/mpeg"
                                    className="file-input file-input-bordered w-full"
                                    onChange={(e) =>
                                        setFile(
                                            e.target.files
                                                ? e.target.files[0]
                                                : null
                                        )
                                    }
                                />
                            </div>

                            <div className="card-actions justify-end mt-6">
                                <button
                                    className={`btn btn-primary ${
                                        isUploading ? "loading" : ""
                                    }`}
                                    onClick={handleUpload}
                                    disabled={isUploading}
                                >
                                    {isUploading
                                        ? "Uploading..."
                                        : "Upload Song"}
                                </button>
                            </div>
                        </div>
                    </div>
                </main>
            </div>
        </div>
    );
}