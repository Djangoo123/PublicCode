import { Home } from "./components/Home";
import { OfficeMapping } from "./components/office/OfficeMapping";
import { MapCreation } from "./components/Administration/MapCreation";
import { Login } from "./components/Login";
import UserAdministration from "./components/Administration/UserAdministration";

const AppRoutes = [
    {
        index: true,
        element: <Home />
    },
    {
        path: '/Office',
        element: <OfficeMapping />
    },
    {
        path: '/Creation',
        element: <MapCreation />
    },
    {
        path: '/Login',
        element: <Login />
    },
    {
        path: '/UserAdministration',
        element: <UserAdministration />
    },
];

export default AppRoutes;
