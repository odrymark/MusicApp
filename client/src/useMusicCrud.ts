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

    return {
        login,
        logout,
        getMe,
        createUser,
    };
}
