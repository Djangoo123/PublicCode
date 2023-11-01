'use client';

import "../app/style/Carousel.css";
import Carousel from "nuka-carousel"
import {
	CardMedia
} from '@mui/material';

import useWindowSize from "../components/Shared/useWindowSize";
import { nullEmptyOrUndefined } from "../components/Shared/VoluntisValidation";
import React from "react";

const CarouselComponent = () => {
    const size = useWindowSize();
    let isMobile = !nullEmptyOrUndefined(size) && !nullEmptyOrUndefined(size.width) && size.width < 800
	let IsIpad = !nullEmptyOrUndefined(size) && !nullEmptyOrUndefined(size.width) && (size.width > 800 && size.width < 1200)

	let numberSlide = 0;
	if (isMobile) numberSlide = 1;
	else if (IsIpad) numberSlide = 2
	else numberSlide = 3

	return (
			<Carousel renderBottomCenterControls={null} 
			slidesToShow={numberSlide} 
			slidesToScroll={1} 
			cellSpacing={isMobile ? 16 : 48} 
			defaultControlsConfig={{ 
				nextButtonStyle: { verticalAlign: 'center', background: "#7A99AC url('/icon-chevron-bas.svg') no-repeat center", opacity: 1}, 
				prevButtonStyle:{ verticalAlign: 'center', background: "#7A99AC url('/icon-chevron-bas.svg') no-repeat center", opacity: 1}, 
				nextButtonText: ' ', 
				prevButtonText: ' ', 
				nextButtonClassName: "nextButton", 
				prevButtonClassName: "previousButton"
			}}>
			{
				items.map((item, index) => {
					return <Banner isMobile={isMobile} item={item} index={index + 1} key={index} />
				})
			}
            </Carousel>
	);
}

// Interface and types items
type Item = {
	Name: any,
	Image: string
}

interface BannerProps {
	isMobile: Boolean
	item: Item,
	index: number
}

const Banner = ({isMobile, item, index}: BannerProps) => {

	return (
		<div className={isMobile ? "Banner" : "Banner_D"} key={item.Name}>
			<CardMedia className={isMobile ? "Media" : "Media_D"} image={item.Image} title={item.Name} />
			<div className="MediaCaption opensans-normal-black-18px">
				<strong>{index}.</strong><br />
				{item.Name}
			</div>
		</div>
	)
}

// Carousel content (img and texts)
const items: Item[] = [
	{
		Name: "",
		Image: ""
	},
	{
		Name: "",
		Image: ""
	},
	{
		Name: "",
		Image: ""
	},
	{
		Name: "",
		Image: ""
	},
	{
		Name: "",
		Image: ""
	},
	{
		Name: "",
		Image: ""
	},
	{
		Name: "",
		Image: ""
	},
	{
		Name: "",
		Image: ""
	},
	{
		Name: "",
		Image: ""
	}
]


export default CarouselComponent;