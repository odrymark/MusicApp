import { useState, useEffect } from "react";
import { useAtomValue } from "jotai";
import { userAtom } from "../atoms/userAtom";
import useMusicCrud, { type Song } from "../useMusicCrud.ts";

export default function MyMusic() {
    const user = useAtomValue(userAtom);
    const { getUserSongs } = useMusicCrud();

    const [songs, setSongs] = useState<Song[]>([]);
    const [search, setSearch] = useState("");
    const [activeTab, setActiveTab] = useState<"songs" | "playlists">("songs");

    useEffect(() => {
        getUserSongs().then((res) => setSongs(res))
    }, [user]);

    const filteredSongs = songs.filter((s) =>
        s.title.toLowerCase().includes(search.toLowerCase())
    );

    return (
        <div className="h-screen flex flex-col overflow-hidden bg-base-200">
            {/* Header */}
            <header className="bg-base-100 shadow-sm border-b border-base-300 flex-shrink-0">
                <div className="navbar px-4 md:px-8 py-3">
                    <span className="text-xl font-bold text-primary ml-2 md:ml-0">
                        Music App
                    </span>
                </div>
            </header>

            <main className="flex-1 overflow-auto p-6 md:p-10 flex flex-col">
                <div className="flex justify-center gap-4 mb-4">
                    <button
                        className={`btn ${activeTab === "songs" ? "btn-primary" : "btn-outline"}`}
                        onClick={() => setActiveTab("songs")}
                    >
                        Songs
                    </button>
                    <button
                        className={`btn ${activeTab === "playlists" ? "btn-primary" : "btn-outline"}`}
                        onClick={() => setActiveTab("playlists")}
                    >
                        Playlists
                    </button>
                </div>

                <div className="flex justify-center mb-4">
                    <input
                        type="text"
                        placeholder="Search songs..."
                        value={search}
                        onChange={(e) => setSearch(e.target.value)}
                        className="input input-bordered w-1/2"
                    />
                </div>

                <div className="flex-1 overflow-y-auto">
                    {activeTab === "songs" && (
                        <div className="grid gap-1 justify-center grid-cols-[repeat(auto-fit,minmax(200px,200px))]">
                            {filteredSongs.length === 0 && (
                                <div className="col-span-full text-center text-base-content/60">
                                    No songs found.
                                </div>
                            )}

                            {filteredSongs.map((song) => (
                                <div
                                    key={song.id}
                                    className="bg-base-100 rounded-lg shadow transition-transform transform hover:scale-105 flex flex-col items-center p-3 aspect-square w-full"
                                >
                                    {song.image ? (
                                        <img
                                            src={song.image}
                                            alt={song.title}
                                            className="w-full h-full object-cover rounded-md mb-2"
                                        />
                                    ) : (
                                        <div className="w-full h-full bg-base-300 rounded-md mb-2 flex items-center justify-center text-base-content/50">
                                            No Image
                                        </div>
                                    )}

                                    <div className="font-semibold text-center text-sm mt-2">
                                        {song.title}
                                    </div>
                                </div>
                            ))}
                        </div>
                    )}

                    {activeTab === "playlists" && (
                        <div className="text-center text-lg text-base-content/70">
                            Playlists content goes here.
                        </div>
                    )}
                </div>
            </main>
        </div>
    );
}