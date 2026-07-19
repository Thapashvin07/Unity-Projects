# Store-Viewer

* An Unity Application that loads products from a json catalogue.
* Has Filter applying to category, sub-cateogory and also to specific items and product search.
* Has detailed panel for product viewing and interaction.



Unity Version : 6000.0.72f



A small overview about the project setup:



* Product Manager a singleton class loads products and owns the productList directly from the local JSON file from Streaming Assets and also has  Current Filter (Filter State). It fires OnCatalogueProductsLoaded, FIlterApplied events which is subscribed by others.
* Product class is defined in ProductManager which contains the main members of the product which is defined in json file.
* FilterState contains current selected category, sub-category and specific items that are selected and also search query for searching query matched products and categories.
* FilterPanelManager manages the ui elements present in the filter panel
* Recycle Scroll script attached to the content of products scroll spawns card and hides them and alters height of rect transform of the content acc to user scrolling using columnCount which is set acc to device , width, height, spacing and how many extra rows kept visible.
* Spawning and Hiding is done with the help of OjectPooler.
* StoreHandler is the heart of main / home screen where action events are subscribed like onproductsloaded, filterapplied, search, and bind card where the ProductCardDisplayer attached in cardPrefab gets called through subscription which does the ui work for the card.
* Inside ProductCardDisplayer when doing UI work calls another singleton TextureCacher which caches texture and sends callbacks for those who called it for getting texture.
* StoreHandler also contains reference to the ProductDetailPanelController where the product details are viewed individually when the card is clicked and it has its ui props.
* The view button in detail panel opens the 3dModel and ModelGestureController script handles rotation, scaling and reset of the model using touch input.
* A camera specially for rendering 3d model has been created with a separate layer ProductView and a render texture of 512x512 has been used for that camera.
* ProductSearch used a very standard approach of searching with matching name and category.
* Downloaded icons and uploaded to my GitHub and downloading it from there using TextureCacher.(/Images outside the Unity\_Assignment folder)
* Have checked in different resolutions and aspects and in several android simulators and it works 90% without issue.



Limitations:



Couldnt implement ProductSorter and effective Search solution due to time constraint but tried search using conventional method.

Couldnt test with more products like 1000.

