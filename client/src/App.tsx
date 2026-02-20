import { createBrowserRouter, type RouteObject, RouterProvider, Navigate } from "react-router-dom";
import Login from "./pages/Login.tsx";
import SignUp from "./pages/SignUp.tsx";
import Home from "./pages/Home.tsx";
import UploadSong from "./pages/UploadSong.tsx";

const routes: RouteObject[] = [
    {
        path: "/",
        element: <Navigate to="/login" replace />
    },
    {
        path: "/login",
        element: <Login/>
    },
    {
        path: "/signup",
        element: <SignUp/>
    },
    {
        path: "/home",
        element: <Home/>
    },
    {
        path: "/uploadSong",
        element: <UploadSong/>
    },
];

function App() {
    return <RouterProvider router={createBrowserRouter(routes)} />;
}

export default App;
