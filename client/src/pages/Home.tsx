import { useRef } from "react";
import SongSection, { type Song } from "../components/SongSection.tsx";

export default function Home() {
    const topTrendingRef = useRef<HTMLDivElement>(null);
    const mostListenedRef = useRef<HTMLDivElement>(null);
    const recommendationsRef = useRef<HTMLDivElement>(null);

    const songs: Song[] = Array.from({ length: 15 }).map((_, i) => ({
        id: i,
        title: `Song ${i + 1}`,
        cover: `https://picsum.photos/200?random=${i + 1}`,
    }));

    const topPlaylists = Array.from({ length: 10 }).map((_, i) => ({
        id: i,
        name: `Playlist ${i + 1}`,
        cover: `https://picsum.photos/100/100?random=${i + 1}`,
    }));

    return (
        <div className="h-screen flex flex-col bg-base-200">
            <header className="bg-base-100 shadow-sm border-b border-base-300 flex-shrink-0">
                <div className="navbar px-4 md:px-8 py-3">
                    <div className="flex-1">
            <span className="text-xl font-bold text-primary ml-2 md:ml-0">
              Music App
            </span>
                    </div>
                </div>
            </header>

            <div className="flex flex-1 overflow-hidden">
                <main className="flex-1 overflow-y-auto p-6 md:p-10">
                    <div className="max-w-6xl mx-auto">
                        <SongSection ref={topTrendingRef} title="Top Trending" songs={songs} />
                        <SongSection ref={mostListenedRef} title="Your Most Listened" songs={songs} />
                        <SongSection ref={recommendationsRef} title="Recommendations For You" songs={songs} />
                    </div>
                </main>

                <aside className="w-72 border-l border-base-300 bg-base-100 p-6 flex-shrink-0 flex flex-col">
                    <h3 className="text-lg font-semibold mb-4 text-center text-primary">
                        Top Playlists
                    </h3>
                    <div className="flex-1 flex flex-col gap-3 overflow-y-auto">
                        {topPlaylists.map((playlist) => (
                            <div
                                key={playlist.id}
                                className="flex items-center gap-3 cursor-pointer hover:bg-base-200 p-2 rounded"
                            >
                                <img
                                    src={playlist.cover}
                                    alt={playlist.name}
                                    className="w-12 h-12 rounded-md object-cover"
                                />
                                <span className="font-medium">{playlist.name}</span>
                            </div>
                        ))}
                    </div>
                </aside>
            </div>
        </div>
    );
}