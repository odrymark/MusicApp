import { useState, useEffect } from "react";
import useMusicCrud, {type Song} from "../useMusicCrud";

export default function CreatePlaylist() {
    const { createPlaylist, getSongs, getSignedUrl } = useMusicCrud();

    const [title, setTitle] = useState("");
    const [image, setImage] = useState<File | null>(null);
    const [imagePreview, setImagePreview] = useState<string | null>(null);
    const [isPublic, setIsPublic] = useState(false);
    const [isSubmitting, setIsSubmitting] = useState(false);

    const [songs, setSongs] = useState<Song[]>([]);
    const [search, setSearch] = useState("");
    const [selectedIds, setSelectedIds] = useState<Set<string>>(new Set());

    useEffect(() => {
        getSongs().then(async (res) => {
            const songsWithUrls = await Promise.all(
                res.map(async (song) => ({
                    ...song,
                    image: song.image ? await getSignedUrl(song.image) : null,
                }))
            );
            setSongs(songsWithUrls);
        }).catch(console.error);
    }, []);

    const filteredSongs = songs.filter(
        (s) =>
            s.title.toLowerCase().includes(search.toLowerCase()) ||
            s.artist.toLowerCase().includes(search.toLowerCase())
    );

    const toggleSong = (id: string) => {
        setSelectedIds((prev) => {
            const next = new Set(prev);
            if (next.has(id)) {
                next.delete(id);
            } else {
                next.add(id);
            }
            return next;
        });
    };

    const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const selected = e.target.files?.[0] ?? null;
        setImage(selected);
        setImagePreview(selected ? URL.createObjectURL(selected) : null);
    };

    const handleSubmit = async () => {
        if (!title.trim()) return alert("Please enter a playlist title.");
        if (selectedIds.size === 0) return alert("Please select at least one song.");

        try {
            setIsSubmitting(true);
            await createPlaylist(title, [...selectedIds], isPublic, image ?? undefined);
            alert("Playlist created!");
            setTitle("");
            setImage(null);
            setImagePreview(null);
            setIsPublic(false);
            setSelectedIds(new Set());
        } catch {
            alert("Failed to create playlist.");
        } finally {
            setIsSubmitting(false);
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

            <main className="flex-1 overflow-hidden p-6">
                <div className="flex gap-6 h-full max-w-5xl mx-auto">

                    <div className="card bg-base-100 shadow-xl w-80 flex-shrink-0 self-start">
                        <div className="card-body">
                            <h2 className="card-title text-primary">Create Playlist</h2>

                            <div className="form-control w-full mt-2">
                                <label className="label">
                                    <span className="label-text">Playlist Title</span>
                                </label>
                                <input
                                    type="text"
                                    placeholder="Enter playlist title"
                                    className="input input-bordered w-full"
                                    value={title}
                                    onChange={(e) => setTitle(e.target.value)}
                                />
                            </div>

                            <div className="form-control w-full mt-4">
                                <label className="label">
                                    <span className="label-text">
                                        Cover Image{" "}
                                        <span className="text-base-content/40">(optional)</span>
                                    </span>
                                </label>
                                <div className="flex justify-center mb-2">
                                    <div className="relative w-36 h-36">
                                        {imagePreview ? (
                                            <img
                                                src={imagePreview}
                                                alt="Cover preview"
                                                className="w-36 h-36 object-cover rounded-xl shadow"
                                            />
                                        ) : (
                                            <div className="w-36 h-36 bg-base-300 rounded-xl flex items-center justify-center text-base-content/40 text-4xl">
                                                🎶
                                            </div>
                                        )}
                                        <label className="absolute inset-0 flex items-center justify-center bg-black/40 rounded-xl opacity-0 hover:opacity-100 transition-opacity cursor-pointer">
                                            <span className="text-white text-xs font-semibold">
                                                {imagePreview ? "Change Image" : "Add Image"}
                                            </span>
                                            <input
                                                type="file"
                                                accept="image/jpeg,image/png,image/webp"
                                                className="hidden"
                                                onChange={handleImageChange}
                                            />
                                        </label>
                                    </div>
                                </div>
                            </div>

                            <div className="form-control mt-2">
                                <label className="label cursor-pointer justify-start gap-4">
                                    <span className="label-text">Visible to Others</span>
                                    <input
                                        type="checkbox"
                                        className="toggle toggle-primary"
                                        checked={isPublic}
                                        onChange={(e) => setIsPublic(e.target.checked)}
                                    />
                                </label>
                            </div>

                            <div className="mt-6">
                                <button
                                    className={`btn btn-primary w-full ${isSubmitting ? "loading" : ""}`}
                                    onClick={handleSubmit}
                                    disabled={isSubmitting}
                                >
                                    {isSubmitting ? "Creating..." : "Create Playlist"}
                                </button>
                            </div>
                        </div>
                    </div>

                    <div className="card bg-base-100 shadow-xl flex-1 overflow-hidden flex flex-col">
                        <div className="card-body flex flex-col overflow-hidden">
                            <div className="flex items-center justify-between flex-shrink-0">
                                <h3 className="font-semibold text-lg text-primary">Add Songs</h3>
                                {selectedIds.size > 0 && (
                                    <span className="badge badge-primary">
                                        {selectedIds.size} selected
                                    </span>
                                )}
                            </div>

                            <input
                                type="text"
                                placeholder="Search by title or artist..."
                                className="input input-bordered w-full mt-2 flex-shrink-0"
                                value={search}
                                onChange={(e) => setSearch(e.target.value)}
                            />

                            <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 gap-3 mt-4 overflow-y-auto flex-1 pr-1">
                                {filteredSongs.length === 0 && (
                                    <p className="col-span-full text-center text-base-content/40 py-8">
                                        No songs found.
                                    </p>
                                )}
                                {filteredSongs.map((song) => {
                                    const selected = selectedIds.has(song.id);
                                    return (
                                        <div
                                            key={song.id}
                                            onClick={() => toggleSong(song.id)}
                                            className={`cursor-pointer rounded-xl border-2 p-2 transition-all ${
                                                selected
                                                    ? "border-primary bg-primary/10"
                                                    : "border-base-300 hover:border-primary/50"
                                            }`}
                                        >
                                            <div className="relative">
                                                {song.image ? (
                                                    <img
                                                        src={song.image}
                                                        alt={song.title}
                                                        className="w-full aspect-square object-cover rounded-lg"
                                                    />
                                                ) : (
                                                    <div className="w-full aspect-square bg-base-300 rounded-lg flex items-center justify-center text-3xl">
                                                        🎵
                                                    </div>
                                                )}
                                                {selected && (
                                                    <div className="absolute inset-0 bg-primary/20 rounded-lg flex items-center justify-center">
                                                        <span className="text-primary text-2xl font-bold">✓</span>
                                                    </div>
                                                )}
                                            </div>
                                            <p className="text-sm font-semibold mt-2 truncate">{song.title}</p>
                                            <p className="text-xs text-base-content/50 truncate">{song.artist}</p>
                                        </div>
                                    );
                                })}
                            </div>
                        </div>
                    </div>

                </div>
            </main>
        </div>
    );
}