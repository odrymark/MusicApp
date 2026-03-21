import { Api } from "./Api.ts";
import type { UserLoginReqDto, UserCreateReqDto } from "./Api.ts";
import { userAtom } from "./atoms/userAtom.ts";
import { useAtom } from "jotai";

const api = new Api({
    baseUrl: "http://localhost:5249",
    baseApiParams: {
        credentials: "include"
    }
});

export type UserInfo = {
    id: string;
    username: string;
    isAdmin: boolean;
};

export type Song = {
    id: string;
    title: string;
    songKey: string;
    url: string;
    artist: string;
    image: string | null;
}

export default function useMusicCrud() {
    const [,setUser] = useAtom(userAtom);

    async function login(dto: UserLoginReqDto): Promise<void> {
        try {
            const response = await api.api.authLogin(dto);

            const userData = await response.json() as unknown as UserInfo;

            setUser(userData);
        } catch (error) {
            console.error("Login failed:", error);
            throw error;
        }
    }

    async function logout(): Promise<void> {
        try {
            await api.api.authLogout();
            setUser(null);
        } catch (error) {
            console.error("Logout failed:", error);
            throw error;
        }
    }

    async function getMe(): Promise<void> {
        try {
            const response = await api.api.authGetMe();

            setUser(await response.json() as unknown as UserInfo);
        } catch {
            setUser(null);
        }
    }

    async function createUser(dto: UserCreateReqDto): Promise<void> {
        try {
            await api.api.userCreateUser(dto);

            await login({username: dto.username, password: dto.password});
        } catch (error) {
            console.error("Creating user failed:", error);
            throw error;
        }
    }

    async function uploadSong(file: File, title: string, artist: string, isPublic: boolean, image?: File): Promise<void> {
        try {
            const formData = new FormData();
            formData.append("file", file);
            formData.append("title", title);
            formData.append("artist", artist);
            formData.append("isPublic", String(isPublic));
            if (image) formData.append("image", image);

            await api.api.songUploadSong(formData as any);
        } catch (error) {
            console.error("Uploading song failed:", error);
            throw error;
        }
    }

    async function getUserSongs(): Promise<Song[]> {
        try {
            const res = await api.api.songGetUserSongs();

            return await res.json() as Song[];
        } catch (error) {
            console.error("Retrieving songs failed:", error);
            throw error;
        }
    }

    async function getSongs(): Promise<Song[]> {
        try {
            const res = await api.api.songGetSongs();

            return await res.json() as Song[];
        } catch (error) {
            console.error("Retrieving songs failed:", error);
            throw error;
        }
    }

    async function getSignedUrl(key: string): Promise<string> {
        try {
            const res = await api.api.songGetSignedUrl({ key });

            return await res.text();
        }
        catch (error) {
            console.error("Retrieving song url failed:", error);
            throw error;
        }
    }

    return {
        login,
        logout,
        getMe,
        createUser,
        uploadSong,
        getUserSongs,
        getSongs,
        getSignedUrl,
    };
}
