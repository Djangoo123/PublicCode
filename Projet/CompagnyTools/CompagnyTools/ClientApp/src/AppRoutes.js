import { Home } from "./components/Home";
import { OfficeMapping } from "./components/OfficeMapping";
import { MapCreation } from "./components/MapCreation";

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
];

export default AppRoutes;
