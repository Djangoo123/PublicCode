import React from 'react';

import Typography from '@mui/material/Typography';

// Generate null/invisible component
// Usefull when you want to add nothing on screen rather than "null" on the render

export default function NullComp(props)
{
    return <Typography style={{ display: 'none' }}> </Typography>;
}
