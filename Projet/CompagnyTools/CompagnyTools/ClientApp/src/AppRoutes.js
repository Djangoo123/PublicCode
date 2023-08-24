import { Home } from "./components/Home";
import { OfficeMapping } from "./components/OfficeMapping";

const AppRoutes = [
  {
    index: true,
    element: <Home />
    },
    {
        path: '/Office',
        element: <OfficeMapping />
    },
];

export default AppRoutes;
