import { useAtomValue } from "jotai";
import { useNavigate, Outlet } from "react-router-dom";
import { userAtom } from "../atoms/userAtom";
import useMusicCrud from "../useMusicCrud";
import {useEffect} from "react";

export default function Sidebar() {
    const { getMe } = useMusicCrud();
    const user = useAtomValue(userAtom);
    const navigate = useNavigate();
    const { logout } = useMusicCrud();

    useEffect(() => {
        getMe().catch(() => {});
    }, []);

    const username = user?.username ?? "Guest";

    return (
        <div className="flex h-screen overflow-hidden bg-base-200">
            {/* Sidebar */}
            <aside className="bg-base-100 border-r border-base-300 w-72 flex-shrink-0 flex flex-col h-full">
                <div className="flex items-center gap-3 p-4">
                    <div className="w-10 h-10 rounded-full overflow-hidden border-2 border-primary">
                        <img
                            src={`https://api.dicebear.com/7.x/avataaars/svg?seed=${username}`}
                            alt="User avatar"
                            className="w-full h-full"
                        />
                    </div>
                    <span className="font-medium text-base-content">{username}</span>
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
                        <button
                            className="flex items-center gap-3 p-3 rounded-lg bg-primary/10 text-primary font-medium"
                            onClick={() => navigate("/home")}
                        >
                            <span className="text-xl">🏠</span>
                            <span>Home</span>
                        </button>
                        {user && (
                            <>
                                <button
                                    className="flex items-center gap-3 p-3 rounded-lg hover:bg-base-200 transition-colors"
                                    onClick={() => navigate("/myMusic")}
                                >
                                    <span className="text-xl">📊</span>
                                    <span>My Music</span>
                                </button>

                                <button
                                    className="flex items-center gap-3 p-3 rounded-lg hover:bg-base-200 transition-colors"
                                    onClick={() => navigate("/uploadSong")}
                                >
                                    <span className="text-xl">⬆️</span>
                                    <span>Upload Song</span>
                                </button>
                            </>
                        )}
                        <button className="flex items-center gap-3 p-3 rounded-lg hover:bg-base-200 transition-colors">
                            <span className="text-xl">⚙️</span>
                            <span>Settings</span>
                        </button>
                    </nav>

                    {user && (
                        <div className="flex justify-center mt-4">
                            <button className="btn btn-primary w-60" onClick={() => logout()}>
                                Logout
                            </button>
                        </div>
                    )}
                </div>
            </aside>

            <main className="flex-1 flex flex-col overflow-hidden">
                <div className="flex-1 overflow-hidden p-6 md:p-10">
                    <Outlet />
                </div>
            </main>
        </div>
    );
}