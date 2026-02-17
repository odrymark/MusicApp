import { useRef, useEffect } from "react";
import { useAtomValue } from "jotai";
import SongSection, { type Song } from "../components/SongSection.tsx";
import { userAtom } from "../atoms/userAtom";
import useMusicCrud from "../useMusicCrud.ts";
import {useNavigate} from "react-router-dom";

export default function Home() {
    const user = useAtomValue(userAtom);
    const navigate = useNavigate();
    const { getMe, logout } = useMusicCrud();

    const username = user?.username ?? "Guest";

    const topTrendingRef = useRef<HTMLDivElement>(null);
    const mostListenedRef = useRef<HTMLDivElement>(null);
    const recommendationsRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        getMe().catch(() => {});
    }, []);

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
                            <a className="flex items-center gap-3 p-3 rounded-lg bg-primary/10 text-primary font-medium">
                                <span className="text-xl">🏠</span>
                                <span>Dashboard</span>
                            </a>
                            <a className="flex items-center gap-3 p-3 rounded-lg hover:bg-base-200 transition-colors">
                                <span className="text-xl">📊</span>
                                <span>Analytics</span>
                            </a>
                            <a className="flex items-center gap-3 p-3 rounded-lg hover:bg-base-200 transition-colors">
                                <span className="text-xl">👥</span>
                                <span>Users</span>
                            </a>
                            <a className="flex items-center gap-3 p-3 rounded-lg hover:bg-base-200 transition-colors">
                                <span className="text-xl">⚙️</span>
                                <span>Settings</span>
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

                <div className="flex-1 flex overflow-hidden">
                    <main className="flex-1 overflow-auto p-6 md:p-10">
                        <div className="max-w-6xl mx-auto">
                            <SongSection ref={topTrendingRef} title="Top Trending" songs={songs} />
                            <SongSection ref={mostListenedRef} title="Your Most Listened" songs={songs} />
                            <SongSection
                                ref={recommendationsRef}
                                title="Recommendations For You"
                                songs={songs}
                            />
                        </div>
                    </main>

                    <aside className="w-72 border-l border-base-300 bg-base-100 p-6 flex-shrink-0 flex flex-col h-full">
                        <h3 className="text-lg font-semibold mb-4 text-center text-primary">
                            Top Playlists
                        </h3>
                        <div className="flex-1 flex flex-col justify-center gap-3 overflow-y-auto">
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
                                    <span className="font-medium">
                                        {playlist.name}
                                    </span>
                                </div>
                            ))}
                        </div>
                    </aside>
                </div>
            </div>
        </div>
    );
}
